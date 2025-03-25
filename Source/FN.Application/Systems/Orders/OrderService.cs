using FN.Application.Systems.Orders.Lib;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Order;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net;

namespace FN.Application.Systems.Orders
{
    public class OrderService : IOrderService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _db;
        private readonly IServiceScopeFactory _scopeFactory;
        public OrderService(IConfiguration configuration,
            IServiceScopeFactory scopeFactory,
            AppDbContext db)
        {
            _configuration = configuration;
            _db = db;
            _scopeFactory = scopeFactory;
        }
        public async Task<ApiResult<int>> CreateOrder(int userId, OrderCreateRequest request)
        {
            try
            {
                var product = await _db.ProductDetails.Include(x => x.ProductPrices).FirstOrDefaultAsync(x => x.ItemId == request.ProductId);
                if (product == null) return new ApiErrorResult<int>("Product not found");
                var basePrice = product.ProductPrices.FirstOrDefault(x => x.PriceType == PriceType.BASE)?.Price;
                var order = new Order
                {
                    UserId = userId,
                    ProductId = request.ProductId,
                    TotalAmount = request.Amount,
                    OrderDate = DateTime.Now,
                    OrderStatus = OrderStatus.PENDING,
                    UnitPrice = basePrice ?? 0,
                };
                _db.Orders.Add(order);
                await _db.SaveChangesAsync();
                return new ApiSuccessResult<int>(order.Id);
            }
            catch (Exception e)
            {
                return new ApiErrorResult<int>(e.Message);
            }

        }
        public async Task<ApiResult<string>> CreatePaymentUrl(PaymentInformationModel model, HttpContext context, int orderId)
        {
            var timeZoneById = TimeZoneInfo.FindSystemTimeZoneById(_configuration["TimeZoneId"]!);
            var timeNow = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, timeZoneById);
            var tick = DateTime.Now.Ticks.ToString();
            var pay = new PayLib();

            string hostName = Dns.GetHostName();
            string clientIpAddress = Dns.GetHostAddresses(hostName).GetValue(0)!.ToString()!;

            //var urlCallBack = _configuration["PaymentCallBack:ReturnUrl"];
            var urlCallBack = "http://localhost:4200/payment-callback";
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
                order.OrderStatus = OrderStatus.COMPLETED;
                _db.Orders.Update(order);
                await _db.SaveChangesAsync();
                return new ApiSuccessResult<string>(paymentUrl);
            }

            return new ApiSuccessResult<string>(paymentUrl);
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

                    var existedOwner = await dbContext.ProductOwners
                        .FirstOrDefaultAsync(x => x.ProductId == productId && x.UserId == userId)
                        .ConfigureAwait(false);

                    if (existedOwner != null)
                    {
                        return new ApiErrorResult<PaymentResponseModel>("Bạn đã mua sản phẩm này rồi");
                    }

                    var payment = new Payment
                    {
                        OrderId = orderId,
                        TransactionId = response.TransactionId,
                        PaymentStatus = response.Success ? PaymentStatus.SUCCESS : PaymentStatus.FAILED,
                        PaymentDate = DateTime.Now,
                        PaymentFee = amount / 100,
                        Description = title,
                        ProductId = productId,
                    };

                    await dbContext.Payments.AddAsync(payment).ConfigureAwait(false);

                    if (response.Success)
                    {
                        var owner = new ProductOwner
                        {
                            ProductId = productId,
                            UserId = userId,
                        };
                        await dbContext.ProductOwners.AddAsync(owner).ConfigureAwait(false);
                    }

                    await dbContext.SaveChangesAsync().ConfigureAwait(false);
                    return new ApiSuccessResult<PaymentResponseModel>(response, "Đang xử lý");
                }
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<PaymentResponseModel>(ex.Message);
            }
        }

    }
}
