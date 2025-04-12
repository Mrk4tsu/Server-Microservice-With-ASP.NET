using AutoMapper;
using AutoMapper.QueryableExtensions;
using FN.Application.Catalog.Product.Pattern;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.FeedbackProduct;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Mailjet.Client.Resources;
using Microsoft.EntityFrameworkCore;

namespace FN.Application.Catalog.Product
{
    public class ProductPublicService : IProductPublicService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IRedisService _dbRedis;
        public ProductPublicService(AppDbContext db, IMapper mapper, IRedisService redis)
        {
            _mapper = mapper;
            _db = db;
            _dbRedis = redis;
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
                        .Where(pp => !product.IsDeleted && pp.EndDate > DateTime.Now),
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
                        .Where(pp => !product.IsDeleted && pp.EndDate > DateTime.Now),
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
        public Task<ApiResult<PagedResult<ProductViewModel>>> GetProductsOwner(ProductPagingRequest request, int userId)
        {
            throw new NotImplementedException();
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
                TimeCreated = DateTime.Now
            };
            _db.FeedBacks.Add(feedback);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(feedback.Id);
        }

        //public async Task<ApiSuccessResult<PagedResult<FeedbackViewModel>>> GetFeedbackProduct(PagedList request, int productId)
        //{
        //    var query = from f in _db.FeedBacks
        //                join u in _db.Users on f.UserId equals u.Id
        //                where f.ProductId == productId && f.Status == true
        //                select new FeedbackViewModel
        //                {
        //                    Id = f.Id,
        //                    Content = f.Content,
        //                    Rate = f.Rate,
        //                    TimeCreated = f.TimeCreated,
        //                    UserName = u.UserName!,
        //                    FullName = u.FullName!,
        //                    UserId = f.UserId,
        //                    Avatar = u.Avatar!
        //                };
        //    var totalRow = await query.CountAsync();
        //    var feedbacks = await query.OrderByDescending(x => x.TimeCreated)
        //        .Skip((request.PageIndex - 1) * request.PageSize)
        //        .Take(request.PageSize)
        //        .ToListAsync();
        //    var pagedResult = new PagedResult<FeedbackViewModel>
        //    {
        //        TotalRecords = totalRow,
        //        PageIndex = request.PageIndex,
        //        PageSize = request.PageSize,
        //        Items = feedbacks
        //    };
        //    return new ApiSuccessResult<PagedResult<FeedbackViewModel>>(pagedResult);
        //}
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
    }
}
