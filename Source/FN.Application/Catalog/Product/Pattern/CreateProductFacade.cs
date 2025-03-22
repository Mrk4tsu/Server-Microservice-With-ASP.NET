using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.Utilities;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using Ganss.Xss;
using Microsoft.EntityFrameworkCore;

namespace FN.Application.Catalog.Product.Pattern
{
    public class CreateProductFacade : BaseService
    {
        public CreateProductFacade(AppDbContext db, IRedisService dbRedis, IImageService image) : base(db, dbRedis, image, SystemConstant.PRODUCT_KEY)
        {
        }
        public async Task<ApiResult<int>> CreateCombine(CombinedCreateOrUpdateRequest request, int userId)
        {
            var executionStrategy = _db.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using var transaction = await _db.Database.BeginTransactionAsync();
                try
                {
                    var itemCreate = new ItemRequest
                    {
                        Title = request.Title,
                        Description = request.Description,
                        Keywords = request.Keywords,
                        Thumbnail = request.Thumbnail
                    };
                    var itemResult = await CreateItemInternal(userId, itemCreate);
                    if (!itemResult.Success) return itemResult;

                    var productDetailCreate = new ProductDetailRequest
                    {
                        Detail = request.Detail,
                        Version = request.Version,
                        Note = request.Note,
                        CategoryId = request.CategoryId,
                        Status = request.Status,
                        NewImages = request.NewImages
                    };
                    var productDetailResult = await CreateProductDetailInternal(itemResult.Data, productDetailCreate);
                    if (!productDetailResult.Success) return productDetailResult;

                    var priceCreate = new ProductPriceRequest
                    {
                        Price = request.Price
                    };
                    var priceResult = await CreatePriceInternal(priceCreate, productDetailResult.Data);
                    if (!priceResult.Success) return priceResult;

                    var imageCreate = new ProductImagesRequest
                    {
                        Images = request.NewImages
                    };
                    var imageResult = await CreateImageInternal(imageCreate, productDetailResult.Data);
                    if (!imageResult.Success) return imageResult;

                    await transaction.CommitAsync();
                    await RemoveOldCache();
                    return new ApiSuccessResult<int>(itemResult.Data);
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    return new ApiErrorResult<int>(ex.ToString());
                }
            });
        }
        private async Task<ApiResult<int>> CreateItemInternal(int userId, ItemRequest request)
        {
            var code = StringHelper.GenerateProductCode(request.Title!);
            var newItem = new Item()
            {
                Title = request.Title!,
                Description = request.Description!,
                Keywords = request.Keywords!,
                SeoTitle = request.Title!,
                UserId = userId,
                CreatedDate = Now(),
                ModifiedDate = Now(),
                NormalizedTitle = StringHelper.NormalizeString(request.Title!),
                SeoAlias = StringHelper.GenerateSeoAlias(request.Title!),
                Code = code,
                Thumbnail = "",
            };
            await _db.Items.AddAsync(newItem);
            await _db.SaveChangesAsync();

            newItem.Thumbnail = await UploadImage(request.Thumbnail!, newItem.Id.ToString(), newItem.Id.ToString()) ?? "";
            _db.Items.Update(newItem);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(newItem.Id);
        }
        private async Task<ApiResult<int>> CreateProductDetailInternal(int itemId ,ProductDetailRequest request)
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedTags.Add("code");
            var productDetail = new ProductDetail()
            {
                Detail = sanitizer.Sanitize(request.Detail!),
                Version = request.Version!,
                Note = request.Note!,
                ItemId = itemId,
                CategoryId = request.CategoryId,
            };
            await _db.ProductDetails.AddAsync(productDetail);
            await _db.SaveChangesAsync();

            return new ApiSuccessResult<int>(productDetail.Id);
        }      
        private async Task<ApiResult<int>> CreatePriceInternal(ProductPriceRequest request, int productDetailId)
        {
            var price = new ProductPrice()
            {
                Price = request.Price!.Value,
                ProductDetailId = productDetailId,
                CreatedDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.MaxValue,
                PriceType = PriceType.BASE
            };
            await _db.ProductPrices.AddAsync(price);
            await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(price.Id);
        }
        private async Task<ApiResult<int>> CreateImageInternal(ProductImagesRequest request, int productId)
        {
            var product = await _db.ProductDetails.Include(x => x.Item).FirstOrDefaultAsync(x => x.Id == productId);
            if (product == null) return new ApiErrorResult<int>("Product not found");
            if (product.ProductImages == null) product.ProductImages = new List<ProductImage>();

            //Nếu không có ảnh thì bỏ qua việc upload ảnh
            if (request.Images == null || !request.Images.Any()) return new ApiSuccessResult<int>(productId); ;

            foreach (var imageFile in request.Images)
            {
                var publicId = _image.GenerateId();
                var imageUpload = await UploadImage(imageFile, publicId, product.ItemId.ToString());
                if (imageUpload == null) return new ApiErrorResult<int>("Upload image failed");
                var newImg = new ProductImage
                {
                    Caption = imageFile.FileName,
                    ImageUrl = imageUpload,
                    PublicId = publicId,
                    ProductDetailId = product.Id
                };
                _db.ProductImages.Add(newImg);
            }
            var result = await _db.SaveChangesAsync();
            return new ApiSuccessResult<int>(result);
        }
    }
}
