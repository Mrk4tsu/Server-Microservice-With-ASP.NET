using AutoMapper;
using FN.Application.Catalog.Product.Pattern;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.ViewModel.Catalog.ProductItems;
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

        public async Task<ApiResult<int>> Create(CombinedCreateOrUpdateRequest request, int userId)
        {
            var facade = new CreateProductFacade(_db, _dbRedis, _image);
            return await facade.CreateCombine(request, userId);
        }
        public async Task<ApiResult<bool>> Update(CombinedCreateOrUpdateRequest request, int itemId, int productId, int userId)
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
        public async Task<ApiResult<bool>> DeleteOrRoleback(int itemId, int userId)
        {
            var item = await _db.Items.FirstOrDefaultAsync(x => x.Id == itemId && x.UserId == userId);
            if (item == null) return new ApiErrorResult<bool>("Không tìm thấy sản phẩm");

            item.IsDeleted = !item.IsDeleted;

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
                if (request.ImageIds != null && request.ImageIds.Any())
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
                return new ApiSuccessResult<bool>();
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

        public async Task<ApiResult<int>> AddItemProduct(ProductItemRequest request, int productId)
        {
            var product = await _db.ProductDetails.FindAsync(productId);
            if (product == null) return new ApiErrorResult<int>("Không tìm thấy sản phẩm");

            if (request.Url != null)
            {
                foreach (var u in request.Url)
                {
                    var item = new ProductItem()
                    {
                        ProductId = productId,
                        Url = u,
                    };
                    await _db.ProductItems.AddAsync(item).ConfigureAwait(false);
                }
            }
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return new ApiSuccessResult<int>(product.Id);
        }

        public async Task<ApiResult<int>> EditItemProduct(ProductItemSingleRequest request, int itemProductId)
        {
            var productItem = await _db.ProductItems.FindAsync(itemProductId);
            if (productItem == null) return new ApiErrorResult<int>("Không tìm thấy sản phẩm");
            if (!string.IsNullOrEmpty(request.Url))
            {
                productItem.Url = request.Url;
            }
            _db.ProductItems.Update(productItem);
            await _db.SaveChangesAsync().ConfigureAwait(false);
            return new ApiSuccessResult<int>(productItem.Id);
        }

        public async Task<ApiResult<PagedResult<ProductOwnerViewModel>>> GetProductsOwner(ProductPagingRequest request, int userId)
        {
            var query = _db.ProductOwners.AsNoTracking().Where(x => x.UserId == userId)
                .Include(x => x.Product)
                .ThenInclude(x => x.Item)
                .ThenInclude(x => x.ProductDetails)
                .ThenInclude(x => x.ProductItems)
                .AsQueryable();
            if (!string.IsNullOrEmpty(request.KeyWord))
            {
                query = query.Where(x => x.Product.Item.Title.Contains(request.KeyWord) || x.Product.Item.Code.Contains(request.KeyWord));
            }
            var totalRow = await query.CountAsync();

            var data = await query.Select(x => new ProductOwnerViewModel()
            {
                Id = x.ProductId,
                ItemId = x.Product.ItemId,
                ProductName = x.Product.Item.Title,
                SeoAlias = x.Product.Item.SeoAlias,
                Price = x.Product.ProductPrices.FirstOrDefault(x => x.PriceType == DataAccess.Enums.PriceType.BASE)!.Price,
                Thumbnail = x.Product.Item.Thumbnail,
                Url = x.Product.ProductItems.Select(x => x.Url).ToList()
            }).OrderByDescending(x => x.ProductName)
            .Skip((request.PageIndex - 1) * request.PageSize)
            .Take(request.PageSize).ToListAsync();

            var result = new PagedResult<ProductOwnerViewModel>
            {
                Items = data,
                TotalRecords = totalRow,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
            };
            return new ApiSuccessResult<PagedResult<ProductOwnerViewModel>>(result);
        }

        public async Task<ApiResult<ManageProductViewModel>> GetProductById(int id)
        {
            var product = await _db.ProductDetails
                .Include(x => x.Category)
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductImages)
                .Include(x => x.ProductItems)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (product == null) return new ApiErrorResult<ManageProductViewModel>("Không tìm thấy sản phẩm");
            var productViewModel = new ManageProductViewModel
            {
                Id = product.ItemId,
                Title = product.Item.Title,
                Description = product.Item.Description,
                Detail = product.Detail,
                Version = product.Version,
                Note = product.Note,
                Keywords = product.Item.Keywords,
                CategoryId = product.CategoryId,
                Thumbnail = product.Item.Thumbnail,
                Price = product.ProductPrices.FirstOrDefault(x => x.PriceType == DataAccess.Enums.PriceType.BASE)!.Price,
                Images = product.ProductImages.Select(x => new ImageProductViewModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl
                }).ToList(),
                Url = product.ProductItems.Select(x => new UrlProductViewModel
                {
                    Id = x.Id,
                    Url = x.Url
                }).ToList()
            };
            return new ApiSuccessResult<ManageProductViewModel>(productViewModel);
        }
    }
}
