using Confluent.Kafka;
using FN.Application.Systems.Events;
using FN.Application.Systems.Kafka;
using FN.Application.Systems.Orders.Lib;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.Utilities;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using FN.ViewModel.Systems.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;

namespace FN.Application.Systems.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ISaleEventService _saleEvent;
        private readonly IKafkaProducer _producer;
        public OrderService(IConfiguration configuration,
            IKafkaProducer producer,
            ISaleEventService saleEvent,
            IServiceScopeFactory scopeFactory,
            AppDbContext db)
        {
            _configuration = configuration;
            _db = db;
            _producer = producer;
            _scopeFactory = scopeFactory;
            _saleEvent = saleEvent;
        }

        public async Task<ApiResult<int>> CreateOrder(int userId, OrderCreateRequest request)
        {
            var user = await _db.Users.FindAsync(userId);
            if (!user!.EmailConfirmed) return new ApiErrorResult<int>("Tài khoản chưa xác thực email");

            var eventPrice = await _saleEvent.GetCurrentEventPrice(request.ProductId);
            decimal finalPrice = await DetermineFinalPrice(request.ProductId, eventPrice);
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                if (finalPrice == 0)
                {
                    await HandleProductFree(userId, request.ProductId);
                    await transaction.CommitAsync();

                    return new ApiSuccessResult<int>();
                }

                var order = new UserOrder
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    TotalAmount = finalPrice,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.PENDING,
                    UnitPrice = request.Amount,

                };
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                return new ApiSuccessResult<int>(order.Id);
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                return new ApiErrorResult<int>(e.Message);
            }

        }
        private async Task HandleProductFree(int userId, int productId)
        {
            var owner = new ProductOwner
            {
                ProductId = productId,
                UserId = userId,
            };
            await _db.ProductOwners.AddAsync(owner).ConfigureAwait(false);
            var product = await _db.ProductDetails.FindAsync(productId);
            product!.DownloadCount += 1;

            await _db.SaveChangesAsync();
        }
        private async Task<decimal> DetermineFinalPrice(int productId, ProductPrice? eventPrice)
        {
            if (eventPrice != null)
            {
                return eventPrice.Price;
            }

            var regularPrice = await _db.ProductPrices
                .Where(pp => pp.ProductDetailId == productId && pp.PriceType == PriceType.BASE)
                .OrderByDescending(pp => pp.CreatedDate)
                .FirstOrDefaultAsync();

            return regularPrice?.Price ?? 0;
        }
        public async Task<ApiResult<string>> CreatePaymentUrl(PaymentInformationModel model, HttpContext context, int orderId)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]!);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new PayLib();

            string hostName = System.Net.Dns.GetHostName();
            string clientIpAddress = System.Net.Dns.GetHostAddresses(hostName).GetValue(0)!.ToString()!;

            var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
            //var urlCallBack = "https://mrkatsu.io.vn/payment-callback";
            pay.AddRequestData("vnp_Version", _configuration["Vnpay:Version"]!);
            pay.AddRequestData("vnp_Command", _configuration["Vnpay:Command"]!);
            pay.AddRequestData("vnp_TmnCode", _configuration["Vnpay:TmnCode"]!);
            pay.AddRequestData("vnp_Amount", ((int)model.Amount * 100).ToString());
            pay.AddRequestData("vnp_CreateDate", timeNow.ToString("yyyyMMddHHmmss"));
            pay.AddRequestData("vnp_CurrCode", _configuration["Vnpay:CurrCode"]!);
            pay.AddRequestData("vnp_IpAddr", clientIpAddress);
            pay.AddRequestData("vnp_Locale", _configuration["Vnpay:Locale"]!);
            pay.AddRequestData("vnp_OrderInfo", $"{model.Name} {model.OrderDescription}");
            pay.AddRequestData("vnp_OrderType", model.OrderType);
            pay.AddRequestData("vnp_ReturnUrl", urlCallBack!);
            pay.AddRequestData("vnp_TxnRef", tick);


            var paymentUrl =
               pay.CreateRequestUrl(_configuration["Vnpay:BaseUrl"]!, _configuration["Vnpay:HashSecret"]!);

            var order = await _db.Orders.FindAsync(orderId);
            if (order != null)
            {
                order.OrderStatus = OrderStatus.WAITING_FOR_PAYMENT;
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
                return new ApiSuccessResult<string>(paymentUrl);
            }

            return new ApiSuccessResult<string>(data: paymentUrl);
        }

        public async Task<ApiResult<PaymentResponseModel>> PaymentExecute(IQueryCollection collections, int userId)
        {
            try
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                    var pay = new VnPayLibrary();
                    var response = pay.GetFullResponseData(collections, _configuration["Vnpay:HashSecret"]!);
                    var amount = long.Parse(collections["vnp_Amount"]!);
                    string[] part = response.OrderDescription.Split(" ");
                    string title = string.Join(" ", part.Take(part.Length - 2));
                    int orderId = int.Parse(part[part.Length - 2]);
                    int productId = int.Parse(part.Last());

                    var order = await dbContext.Orders.FindAsync(orderId);
                    order!.OrderStatus = response.Success ? OrderStatus.COMPLETED : OrderStatus.PAYMENT_FAILED;
                    var eventProduct = await _db.SaleEventProducts.Include(x =>x.SaleEvent)
                           .FirstOrDefaultAsync(x => x.ProductDetailId == order.ProductId && x.SaleEvent.IsActive);

                    var payment = new Payment
                    {
                        OrderId = orderId,
                        UserId = userId,
                        TransactionId = response.TransactionId,
                        PaymentStatus = response.Success ? PaymentStatus.SUCCESS : PaymentStatus.FAILED,
                        PaymentDate = DateTime.Now,
                        PaymentFee = amount / 100,
                        Description = title,
                        ProductId = productId,
                    };

                    dbContext.Payments.Add(payment);

                    if (response.Success)
                    {
                        var owner = new ProductOwner
                        {
                            ProductId = productId,
                            UserId = userId,
                        };
                         dbContext.ProductOwners.Add(owner);
                        var product = await dbContext.ProductDetails.FindAsync(productId);
                        product!.DownloadCount += 1;
 

                        if (eventProduct != null)
                        {
                            var eventMessage = new EventMessage
                            {
                                ProductId = eventProduct.Id,
                                UserId = userId,
                            };
                            await _producer.Produce(SystemConstant.EVENT_PAYMENT_TOPIC_KAFKA, new Message<string, string>
                            {
                                Key = payment.TransactionId.ToString(),
                                Value = JsonSerializer.Serialize(eventMessage)
                            }).ConfigureAwait(false);
                        }
                    }


                    await dbContext.SaveChangesAsync();
                    return new ApiSuccessResult<PaymentResponseModel>(response, "Đang xử lý");
                }
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<PaymentResponseModel>(ex.Message);
            }
        }

        public async Task<ApiResult<PagedResult<OrderViewModel>>> GetOrders(int userId, OrderPagingRequest request)
        {
            var query = _db.Orders.AsNoTracking().Where(x => x.UserId == userId);

            if (!string.IsNullOrEmpty(request.ProductCode))
                query = query.Include(x => x.Product).Where(x => x.Product.Item.Code.Contains(request.ProductCode));
            if (request.FromDate != default(DateTime) && request.ToDate != default(DateTime))
                query = query.Where(x => x.OrderDate >= request.FromDate && x.OrderDate <= request.ToDate);
            if (request.MaxPrice > 0)
                query = query.Where(x => x.TotalAmount <= request.MaxPrice);
            if (request.MinPrice > 0)
                query = query.Where(x => x.TotalAmount >= request.MinPrice);

            var totalRecord = await query.CountAsync();

            var order = await query.OrderByDescending(x => x.OrderDate)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new OrderViewModel
                {
                    CreatedDate = x.OrderDate,
                    OrderId = x.Id,
                    ProductImage = x.Product.Item.Thumbnail,
                    ProductName = x.Product.Item.Title,
                    Status = x.OrderStatus,
                    TotalPrice = x.TotalAmount
                })
                .ToListAsync();

            var result = new PagedResult<OrderViewModel>()
            {
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = order,
                TotalRecords = totalRecord,
            };
            return new ApiSuccessResult<PagedResult<OrderViewModel>>(result);
        }

        public async Task<ApiResult<PagedResult<PaymentViewModel>>> GetPayments(int userId, PaymentPagingRequest request)
        {
            var query = _db.Payments.AsNoTracking()
                .Where(x => x.UserId == userId);

            if (request.FromDate != default(DateTime) && request.ToDate != default(DateTime))
                query = query.Where(x => x.PaymentDate >= request.FromDate && x.PaymentDate <= request.ToDate);
            if (request.MaxPrice > 0)
                query = query.Where(x => x.PaymentFee <= request.MaxPrice);
            if (request.MinPrice > 0)
                query = query.Where(x => x.PaymentFee >= request.MinPrice);

            var totalRecord = await query.CountAsync();
            var payments = await query.OrderByDescending(x => x.PaymentFee)
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Include(x => x.Product).Select(x => new PaymentViewModel
                {
                    PaymentDate = x.PaymentDate,
                    OrderId = x.OrderId,
                    ProductImage = x.Product.Item.Thumbnail,
                    ProductName = x.Product.Item.Title,
                    PaymentStatus = x.PaymentStatus,
                    PaymentFee = x.PaymentFee,
                    Author = x.Product.Item.User.FullName,
                    DiscountPrice = x.Order.TotalAmount < x.Order.UnitPrice ? x.Order.TotalAmount : x.Order.UnitPrice,
                    UnitPrice = x.Order.UnitPrice,
                    CategoryName = x.Product.Category.Name
                })
                .ToListAsync();
            var result = new PagedResult<PaymentViewModel>()
            {
                PageSize = request.PageSize,
                PageIndex = request.PageIndex,
                Items = payments,
                TotalRecords = totalRecord,
            };
            return new ApiSuccessResult<PagedResult<PaymentViewModel>>(result);
        }
    }
}
public class EventMessage
{
    public int ProductId { get; set; }
    public int UserId { get; set; }
}