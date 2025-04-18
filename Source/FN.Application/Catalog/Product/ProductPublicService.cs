using AutoMapper;
using FN.Application.Catalog.Product.Notifications;
using FN.Application.Catalog.Product.Pattern;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.FeedbackProduct;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace FN.Application.Catalog.Product
{
    public class ProductPublicService : IProductPublicService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IRedisService _dbRedis;
        private readonly ProductContext _context;
        private readonly IProductStrategyFactory _strategyFactory;
        private IHubContext<NotifyHub, ITypedHubClient> _hubContext;
        private readonly INotifyService _notifyService;
        private DateTime _now;
        public ProductPublicService(
            INotifyService notifyService,
            AppDbContext db,
            IMapper mapper,
            ProductContext context,
            IProductStrategyFactory strategyFactory,
            IHubContext<NotifyHub, ITypedHubClient> hubContext,
        IRedisService redis)
        {
            _mapper = mapper;
            _db = db;
            _dbRedis = redis;
            _context = context;
            _strategyFactory = strategyFactory;
            _hubContext = hubContext;
            _notifyService = notifyService;
            _now = new TimeHelper.Builder()
               .SetTimestamp(DateTime.UtcNow)
               .SetTimeZone("SE Asia Standard Time").Build();
        }
        public async Task<ApiResult<ProductDetailViewModel>> GetProduct(int productId, int userId)
        {
            var query = _db.ProductDetails
                .Where(x => x.Id == productId && x.Item.IsDeleted == false)
                .Select(product => new
                {
                    Product = product,
                    Item = product.Item,
                    User = product.Item.User,
                    Category = product.Category,
                    ProductPrices = product.ProductPrices
                        .Where(pp => !product.Item.IsDeleted && pp.EndDate > _now),
                    ProductImages = product.ProductImages,
                    IsOwner = product.Item.UserId == userId ||
                             product.ProductOwners.Any(po => po.UserId == userId),
                    Interaction = product.UserProductInteractions
                        .FirstOrDefault(upi => upi.UserId == userId)
                });

            var result = await query.FirstOrDefaultAsync();

            if (result == null)
                return new ApiErrorResult<ProductDetailViewModel>("Không tìm thấy sản phẩm");

            var detailVM = _mapper.Map<ProductDetailViewModel>(result.Product);
            detailVM.IsOwned = result.IsOwner;
            detailVM.IsInteractive = result.Interaction?.Type ?? DataAccess.Enums.InteractionType.None;

            return new ApiSuccessResult<ProductDetailViewModel>(detailVM);
        }
        public async Task<ApiResult<ProductDetailViewModel>> GetProductWithoutLogin(int productId)
        {
            var query = _db.ProductDetails
                .Where(x => x.Id == productId && x.Item.IsDeleted == false)
                .Select(product => new
                {
                    Product = product,
                    Item = product.Item,
                    User = product.Item.User,
                    Category = product.Category,
                    ProductPrices = product.ProductPrices
                        .Where(pp => !product.Item.IsDeleted && pp.EndDate > _now),
                    ProductImages = product.ProductImages
                });
            var result = await query.FirstOrDefaultAsync();
            if (result == null)
                return new ApiErrorResult<ProductDetailViewModel>("Không tìm thấy sản phẩm");
            var detailVM = _mapper.Map<ProductDetailViewModel>(result.Product);
            detailVM.IsOwned = false;
            detailVM.IsInteractive = DataAccess.Enums.InteractionType.None;

            return new ApiSuccessResult<ProductDetailViewModel>(detailVM);
        }
        public async Task<ApiResult<PagedResult<ProductViewModel>>> GetProducts(ProductPagingRequest request)
        {
            var facade = new GetProductFacade(_db, _dbRedis!, null!, _mapper);
            return await facade.GetProducts(request, false, false, null);
        }

        public async Task<ApiResult<int>> AddProductFeedback(FeedbackRequest request, int userId)
        {
            var check = await _db.FeedBacks
                .FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.UserId == userId);
            if (check != null)
                return new ApiErrorResult<int>("Bạn đã đánh giá sản phẩm này rồi");
            var product = await _db.ProductDetails
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .FirstOrDefaultAsync(x => x.Id == request.ProductId && x.Item.IsDeleted == false);
            var ownerProduct = await _db.ProductOwners
                .FirstOrDefaultAsync(x => x.ProductId == request.ProductId && x.UserId == userId);
            if (ownerProduct == null && product.Item.UserId != userId)
                return new ApiErrorResult<int>("Bạn chưa sở hữu sản phẩm này");

            var feedback = new FeedBack
            {
                ProductId = request.ProductId,
                UserId = userId,
                Content = request.Content,
                Rate = request.Rate,
                Status = true,
                TimeCreated = _now
            };
            _db.FeedBacks.Add(feedback);
            await _db.SaveChangesAsync();

            // Gửi thông báo chỉ cho chủ sở hữu sản phẩm
            var user = await _db.Users.FindAsync(userId);
            var ownerUserId = product.Item.UserId.ToString();
            var ownerConnections = NotifyHub.GetUserConnections(ownerUserId);
            var notifyString = $"Sản phẩm {product.Item.Title} vừa có đánh giá mới từ {user.FullName}";
            var info = new Notifications.Notification
            {
                Content = notifyString,
                Title = "Đánh giá sản phẩm",
                Time = _now,
                Url = $"/product/{product.Item.SeoAlias}-{product.Id}",
            };
            if (ownerConnections.Any())
            {            
                var message = new Notifications.Message()
                {
                    Type = "product",
                    Information = info
                };
                await _hubContext.Clients.Clients(ownerConnections).SendMessage(message);
            }
            await _notifyService.SaveNotify(ownerUserId, info);
            return new ApiSuccessResult<int>(feedback.Id);
        }

        public async Task<ApiSuccessResult<PagedResult<FeedbackViewModel>>> GetFeedbackProduct(PagedList request, int productId)
        {
            var query = _db.FeedBacks
                .Where(f => f.ProductId == productId && f.Status == true)
                .Select(f => new
                {
                    Feedback = f,
                    UserName = f.User.UserName,
                    FullName = f.User.FullName,
                    Avatar = f.User.Avatar
                })
                .OrderByDescending(x => x.Feedback.TimeCreated);

            var totalRow = await query.CountAsync();

            var feedbacks = await query
                .Skip((request.PageIndex - 1) * request.PageSize)
                .Take(request.PageSize)
                .Select(x => new FeedbackViewModel
                {
                    Id = x.Feedback.Id,
                    UserId = x.Feedback.UserId,
                    UserName = x.UserName,
                    FullName = x.FullName,
                    Content = x.Feedback.Content,
                    Rate = x.Feedback.Rate,
                    TimeCreated = x.Feedback.TimeCreated,
                    Avatar = x.Avatar
                })
                .ToListAsync();

            var pagedResult = new PagedResult<FeedbackViewModel>
            {
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                Items = feedbacks
            };

            return new ApiSuccessResult<PagedResult<FeedbackViewModel>>(pagedResult);
        }

        public async Task<ApiResult<List<ProductViewModel>>> GetProducts(string type, int take)
        {
            var strategy = _strategyFactory.GetStrategy(type);
            _context.SetStrategy(strategy);
            var result = await _context.GetProductsSelection(take);
            return new ApiSuccessResult<List<ProductViewModel>>(result);
        }

        public async Task<ApiResult<int>> UpdateView(int productId)
        {
            var product = await _db.Items.FirstOrDefaultAsync(x => x.Id == productId && !x.IsDeleted);
            if (product == null)
                return new ApiErrorResult<int>("Không tìm thấy sản phẩm");
            product.ViewCount += 1;
            _db.Items.Update(product);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(product.ViewCount);
        }
    }
}
