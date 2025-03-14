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
        private const string ROOT = "product";
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
            var facade = new GetProductFacade(_db, _dbRedis, _image);
            return await facade.GetProducts(request, true, false, userId);
        }
        public async Task<ApiResult<PagedResult<ProductViewModel>>> TrashProducts(ProductPagingRequest request, int userId)
        {
            var facade = new GetProductFacade(_db, _dbRedis, _image);
            return await facade.GetProducts(request, true, true, userId);
        }

        public async Task RemoveCacheData()
        {
            await _dbRedis.RemoveValue(SystemConstant.CACHE_PRODUCT);
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
                return new ApiSuccessResult<bool>(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiErrorResult<bool>("Xóa ảnh thất bại: " + ex.Message);
            }
        }
        public async Task<ApiResult<bool>> UpdateCombined(CombinedUpdateRequest request, int itemId, int productId, int userId)
        {
            // Cập nhật Item
            var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == itemId && x.UserId == userId);
            if (item == null) return new ApiErrorResult<bool>("Không tìm thấy sản phẩm");
            if (!string.IsNullOrEmpty(request.Title))
            {
                string code = StringHelper.GenerateProductCode(request.Title);
                item.Title = request.Title;
                item.Code = code;
                item.SeoAlias = StringHelper.GenerateSeoAlias(request.Title);
                item.SeoTitle = request.Title;
                item.NormalizedTitle = StringHelper.NormalizeString(request.Title);
            }
            if (!string.IsNullOrEmpty(request.Description))
                item.Description = request.Description;
            if (!string.IsNullOrEmpty(request.Keywords))
                item.Keywords = request.Keywords;
            if (request.Thumbnail != null)
            {
                string? newThumbnail = await _image.UploadImage(request.Thumbnail, item.Code, Folder(item.Id.ToString()));
                if (!string.IsNullOrEmpty(newThumbnail)) item.Thumbnail = newThumbnail;
            }
            item.ModifiedDate = DateTime.Now;
            _db.Items.Update(item);

            // Cập nhật ProductDetail
            var product = await _db.ProductDetails.FirstOrDefaultAsync(x => x.ItemId == itemId && x.Id == productId);
            if (product == null) return new ApiErrorResult<bool>("Không tìm thấy chi tiết sản phẩm");
            product.Status = request.Status;
            if (!string.IsNullOrEmpty(request.Detail))
                product.Detail = request.Detail;
            if (!string.IsNullOrEmpty(request.Note))
                product.Note = request.Note;
            if (!string.IsNullOrEmpty(request.Version))
                product.Version = request.Version;
            product.CategoryId = request.CategoryId;
            _db.ProductDetails.Update(product);

            if (request.NewImages != null)
            {
                foreach (var file in request.NewImages)
                {
                    var publicId = _image.GenerateId();
                    var resultUpload = await _image.UploadImage(file, publicId, Folder(item.Id.ToString()));
                    if (!string.IsNullOrEmpty(resultUpload))
                    {
                        var newImage = new ProductImage
                        {
                            Caption = file.FileName,
                            ImageUrl = resultUpload,
                            PublicId = publicId,
                            ProductDetailId = product.Id
                        };
                        _db.ProductImages.Add(newImage);
                    }

                }
            }
            // Lưu thay đổi vào cơ sở dữ liệu
            await _db.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true);
        }
        string Folder(string code)
        {
            return $"{ROOT}/{code}";
        }

    }
}
