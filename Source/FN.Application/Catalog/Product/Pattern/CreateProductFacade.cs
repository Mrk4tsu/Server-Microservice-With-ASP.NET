﻿using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.Utilities;
using FN.ViewModel.Catalog;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Helper.API;
using Ganss.Xss;

namespace FN.Application.Catalog.Product.Pattern
{
    public class CreateProductFacade : BaseService
    {
        public CreateProductFacade(AppDbContext db, IRedisService dbRedis, IImageService image) : base(db, dbRedis, image)
        {
        }

        public async Task<ApiResult<int>> Create(CreateProductRequest request, int userId)
        {
            using var transaction = await _db.Database.BeginTransactionAsync();
            try
            {
                var item = await CreateItem(request, userId);
                var productDetail = await CreateProductDetail(request, item.Id);
                await CreatePrice(request, productDetail.Id);
                await CreateImage(request, productDetail, item);
                await transaction.CommitAsync();
                await RemoveOldCache();
                return new ApiSuccessResult<int>(item.Id);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return new ApiErrorResult<int>(ex.ToString());
            }
        }
        private async Task<Item> CreateItem(CreateProductRequest request, int userId)
        {
            var code = StringHelper.GenerateProductCode(request.Title);
            var folder = Folder(code);
            
            var thumbnail = await _image.UploadImage(request.Thumbnail, code, folder);
            var newItem = new Item()
            {
                Title = request.Title,
                Description = request.Description,
                Keywords = request.Keywords,
                SeoTitle = request.Title,
                UserId = userId,
                CreatedDate = Now(),
                ModifiedDate = Now(),
                NormalizedTitle = StringHelper.NormalizeString(request.Title),
                SeoAlias = StringHelper.GenerateSeoAlias(request.Title),
                Code = code,
                Thumbnail = thumbnail ?? "",
            };

            await _db.Items.AddAsync(newItem);
            await _db.SaveChangesAsync();

            return newItem;
        }
        private async Task<ProductDetail> CreateProductDetail(CreateProductRequest request, int itemId)
        {
            var sanitizer = new HtmlSanitizer();
            sanitizer.AllowedAttributes.Add("class");
            sanitizer.AllowedTags.Add("code");
            var productDetail = new ProductDetail()
            {
                Detail = sanitizer.Sanitize(request.Detail),
                Version = request.Version,
                Note = request.Note,
                ItemId = itemId,
                CategoryId = request.CategoryId,
            };
            await _db.ProductDetails.AddAsync(productDetail);
            await _db.SaveChangesAsync();
            return productDetail;
        }
        private async Task CreatePrice(CreatePricesRequest request, int productDetailId)
        {
            var price = new ProductPrice()
            {
                Price = request.Price,
                ProductDetailId = productDetailId,
                CreatedDate = DateTime.UtcNow,
                StartDate = DateTime.UtcNow,
                EndDate = DateTime.MaxValue,
                PriceType = PriceType.BASE
            };

            await _db.ProductPrices.AddAsync(price);
            await _db.SaveChangesAsync();
        }
        private async Task CreateImage(CreateImagesRequest request, ProductDetail product, Item item)
        {
            if (product.ProductImages == null)
            {
                product.ProductImages = new List<ProductImage>();
            }

            if (request.Images != null && request.Images.Count > 0)
            {
                foreach (var imageFile in request.Images)
                {
                    var publicId = _image.GenerateId();
                    var resultUpload = await _image.UploadImage(imageFile, publicId, Folder(item.Code));
                    if (resultUpload == null) throw new Exception("Upload ảnh sản phẩm không thành công");

                    product.ProductImages.Add(new ProductImage
                    {
                        Caption = imageFile.FileName,
                        ImageUrl = resultUpload,
                        PublicId = publicId,
                        ProductDetailId = product.Id
                    });
                }
            }

            await _db.SaveChangesAsync();
        }
    }
}
