using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using Ganss.Xss;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace FN.Application.Catalog.Product.Pattern
{
    public class UpdateProductFacade : BaseService
    {
        public UpdateProductFacade(AppDbContext db, IHttpClientFactory httpClientFactory, IRedisService dbRedis, IImageService image, string root) : base(db, httpClientFactory, dbRedis, image, SystemConstant.PRODUCT_KEY)
        {
        }

        public async Task<ApiResult<bool>> UpdateCombined(CombinedCreateOrUpdateRequest request, int itemId, int productId, int userId)
        {
            var executionStrategy = _db.Database.CreateExecutionStrategy();

            return await executionStrategy.ExecuteAsync(async () =>
            {
                using (var transaction = await _db.Database.BeginTransactionAsync())
                {
                    try
                    {
                        // Cập nhật Item
                        var itemUpdateDto = new ItemRequest
                        {
                            Title = request.Title,
                            Description = request.Description,
                            Keywords = request.Keywords,
                            Thumbnail = request.Thumbnail
                        };
                        var itemResult = await UpdateItemInternal(itemUpdateDto, itemId, userId);
                        if (!itemResult.Success) return itemResult;

                        // Cập nhật ProductDetail
                        var productDetailUpdateRequest = new ProductDetailRequest
                        {
                            Detail = await ProcessContentImages(request.Detail!, itemId),
                            Version = request.Version,
                            Note = request.Note,
                            CategoryId = request.CategoryId,
                            Status = request.Status,
                            NewImages = request.NewImages // Truyền danh sách ảnh mới
                        };
                        var productResult = await UpdateProductDetailInternal(productDetailUpdateRequest, itemId, productId);
                        if (!productResult.Success) return productResult;

                        await transaction.CommitAsync();
                        await RemoveOldCache();
                        return new ApiSuccessResult<bool>(true);
                    }
                    catch (Exception ex)
                    {
                        await transaction.RollbackAsync();
                        return new ApiErrorResult<bool>("Có lỗi xảy ra khi cập nhật: " + ex.Message);
                    }
                }
            });
        }

        private async Task<ApiResult<bool>> UpdateItemInternal(ItemRequest request, int itemId, int userId)
        {
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
                string? newThumbnail = await UploadImage(request.Thumbnail, item.Id.ToString(), item.Id.ToString());
                string? newCover = await HandleCoverImage(request.Thumbnail, item.Id.ToString(), $"cover{item.Id.ToString()}");
                if (!string.IsNullOrEmpty(newThumbnail))
                {
                    item.Thumbnail = newThumbnail;
                    item.Cover = newCover;
                }
            }
            item.ModifiedDate = Now();
            _db.Items.Update(item);
            await _db.SaveChangesAsync();

            return new ApiSuccessResult<bool>();
        }

        private async Task<ApiResult<bool>> UpdateProductDetailInternal(ProductDetailRequest request, int itemId, int productId)
        {
            var productDetail = await _db.ProductDetails
                                        .Include(pd => pd.Item) // Đảm bảo load thông tin Item liên quan
                                        .FirstOrDefaultAsync(x => x.ItemId == itemId && x.Id == productId);
            if (productDetail == null) return new ApiErrorResult<bool>("Không tìm thấy chi tiết sản phẩm");

            productDetail.Status = request.Status;
            if (!string.IsNullOrEmpty(request.Detail))
                productDetail.Detail = ProcessSantizer(request.Detail);
            if (!string.IsNullOrEmpty(request.Note))
                productDetail.Note = request.Note;
            if (!string.IsNullOrEmpty(request.Version))
                productDetail.Version = request.Version;
            productDetail.CategoryId = request.CategoryId;

            // Xử lý thêm ảnh mới
            if (request.NewImages != null && request.NewImages.Any())
            {
                await AddProductImagesAsync(request.NewImages, productDetail);
            }

            _db.ProductDetails.Update(productDetail);
            await _db.SaveChangesAsync();

            return new ApiSuccessResult<bool>(true);
        }

        private async Task AddProductImagesAsync(List<IFormFile> newImages, ProductDetail productDetail)
        {
            foreach (var file in newImages)
            {
                var publicId = _image.GenerateId();
                var resultUpload = await UploadImage(file, publicId, productDetail.ItemId.ToString());
                if (!string.IsNullOrEmpty(resultUpload))
                {
                    var newImage = new ProductImage
                    {
                        Caption = file.FileName,
                        ImageUrl = resultUpload,
                        PublicId = publicId,
                        ProductDetailId = productDetail.Id
                    };
                    _db.ProductImages.Add(newImage);
                }
            }
            await _db.SaveChangesAsync();
        }


    }
}
