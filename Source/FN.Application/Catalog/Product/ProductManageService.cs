using AutoMapper;
using FN.Application.Catalog.Product.Pattern;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FN.Application.Catalog.Product
{
    public class ProductManageService : IProductManageService
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;
        private readonly IImageService _image;
        private readonly IRedisService _dbRedis;
        private readonly IConfiguration _config;
        public ProductManageService(AppDbContext db,
            IConfiguration configuration,
            IRedisService redisService, IMapper mapper, IImageService image)
        {
            _dbRedis = redisService;
            _db = db;
            _mapper = mapper;
            _image = image;
            _config = configuration;
        }

        public async Task<ApiResult<int>> Create(CreateProductRequest request, int userId)
        {
            var facade = new CreateProductFacade(_db, _dbRedis, _image);
            return await facade.Create(request, userId);
        }
        public async Task<ApiResult<bool>> Update(CombinedUpdateRequest request, int itemId, int productId, int userId)
        {
            var facade = new UpdateProductFacade(_db, _dbRedis, _image);
            return await facade.UpdateCombined(request, itemId, productId, userId);
        }
        public async Task<ApiResult<PagedResult<ProductViewModel>>> GetProducts(ProductPagingRequest request, int userId)
        {
            var facade = new GetProductFacade(_db, _dbRedis, _image, _mapper);
            return await facade.GetProducts(request, true, false, userId);
        }
        public async Task<ApiResult<PagedResult<ProductViewModel>>> TrashProducts(ProductPagingRequest request, int userId)
        {
            var facade = new GetProductFacade(_db, _dbRedis, _image, _mapper);
            return await facade.GetProducts(request, true, true, userId);
        }
        public async Task RemoveCacheData()
        {
            await _dbRedis.RemoveValue(SystemConstant.PRODUCT_KEY);
        }
        public async Task<ApiResult<bool>> DeletePermanently(int itemId, int userId)
        {
            var product = await _db.Items
               .Where(x => x.Id == itemId)
               .Select(x => new
               {
                   x.Id,
                   x.Code,
                   ProductDetails = x.ProductDetails!.Select(pd => new
                   {
                       pd.Id,
                       ProductImages = pd.ProductImages.Select(pi => pi.PublicId).ToList()
                   }).ToList()
               })
               .FirstOrDefaultAsync();

            if (product == null)
                return new ApiErrorResult<bool>("Không tìm thấy sản phẩm");

            var folder = (product.Id.ToString());

            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var deleteImageTasks = new List<Task>();

                deleteImageTasks.Add(_image.DeleteImageInFolder(product.Code, folder)); // Xóa thumbnail

                foreach (var productDetail in product.ProductDetails)
                {
                    foreach (var imageId in productDetail.ProductImages)
                    {
                        deleteImageTasks.Add(_image.DeleteImageInFolder(imageId, folder));
                    }
                }
                await Task.WhenAll(deleteImageTasks);
                await _image.DeleteFolderImage(folder);
                var productDetails = await _db.ProductDetails
                    .Where(pd => pd.ItemId == product.Id)
                    .ToListAsync();

                _db.ProductDetails.RemoveRange(productDetails);

                var item = await _db.Items
                    .Where(x => x.Id == product.Id)
                    .FirstOrDefaultAsync();

                if (item != null)
                {
                    _db.Items.Remove(item);
                }

                await _db.SaveChangesAsync();
                await RemoveCacheData();
                await transaction.CommitAsync();
                return new ApiSuccessResult<bool>();
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                throw new Exception("Xóa sản phẩm thất bại", ex);
            }
        }
        public async Task<ApiResult<bool>> Delete(int itemId, int userId)
        {
            var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == itemId && x.UserId == userId);
            if (item == null) return new ApiErrorResult<bool>("Không tìm thấy sản phẩm");

            item.IsDeleted = true;

            _db.Items.Update(item);
            await _db.SaveChangesAsync();
            await RemoveCacheData();
            return new ApiSuccessResult<bool>();
        }
        public async Task<ApiResult<bool>> DeleteImage(DeleteProductImagesRequest request)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var imagesToDelete = await _db.ProductImages.Include(x => x.ProductDetail)
                    .ThenInclude(x => x.Item)
                    .Where(pi => request.ImageIds.Contains(pi.Id)).ToListAsync();
                if (imagesToDelete == null || !imagesToDelete.Any()) return new ApiErrorResult<bool>("Không tìm thấy ảnh cần xóa.");

                foreach (var image in imagesToDelete)
                {
                    await _image.DeleteImageInFolder(image.PublicId, Folder(image.ProductDetail.ItemId.ToString()));
                    _db.ProductImages.Remove(image);
                }

                await _db.SaveChangesAsync();
                await transaction.CommitAsync();
                await RemoveCacheData();
                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiErrorResult<bool>("Xóa ảnh thất bại: " + ex.Message);
            }
        }
        string Folder(string code)
        {
            return $"{SystemConstant.PRODUCT_KEY}/{code}";
        }

    }
}
