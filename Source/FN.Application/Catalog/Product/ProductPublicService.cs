﻿using AutoMapper;
using FN.Application.Catalog.Product.Pattern;
using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.Manage;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
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
        //public async Task<ApiResult<ProductDetailViewModel>> GetProduct(int itemId)
        //{
        //    var product = await _db.ProductDetails
        //        .Where(x => x.ItemId == itemId)
        //        .ProjectTo<ProductDetailViewModel>(_mapper.ConfigurationProvider)
        //        .FirstOrDefaultAsync();

        //    if (product == null)
        //        return new ApiErrorResult<ProductDetailViewModel>("Không tìm thấy sản phẩm");

        //    return new ApiSuccessResult<ProductDetailViewModel>(product);
        //}
        public async Task<ApiResult<ProductDetailViewModel>> GetProduct(int itemId, int userId)
        {
            bool flagOwner = false;

            var product = await _db.ProductDetails
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .Where(x => x.ItemId == itemId && x.Item.IsDeleted == false)
                .Include(x => x.Category)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync();
            if (product == null) return new ApiErrorResult<ProductDetailViewModel>("Không tìm thấy sản phẩm");


            var ownerProduct = await _db.ProductOwners.FirstOrDefaultAsync(x => x.ProductId == product.Id && x.UserId == userId);
            var interaction = await _db.UserProductInteractions
               .Where(x => x.ProductId == product.Id && x.UserId == userId)
               .FirstOrDefaultAsync();
            if (ownerProduct != null || product.Item.UserId == userId) flagOwner = true;
            var detailVM = new ProductDetailViewModel
            {
                Id = product.Item.Id,
                ProductId = product.Id,
                CategoryIcon = product.Category.SeoImage,
                Title = product.Item.Title,
                Detail = product.Detail,
                LikeCount = product.LikeCount,
                DisLikeCount = product.DislikeCount,
                DownloadCount = product.DownloadCount,
                Version = product.Version,
                Note = product.Note,
                IsOwned = flagOwner,
                CategoryName = product.Category.Name,
                SeoAlias = product.Item.SeoAlias,
                TimeCreates = product.Item.CreatedDate,
                TimeUpdates = product.Item.ModifiedDate,
                CategorySeoAlias = product.Category.SeoAlias,
                Description = product.Item.Description,
                Thumbnail = product.Item.Thumbnail,
                Username = product.Item.User.UserName!,
                Author = product.Item.User.FullName,
                ViewCount = product.Item.ViewCount,
                Prices = product.ProductPrices
                        .Where(pp => !pp.ProductDetail.IsDeleted && pp.EndDate > DateTime.Now) // Lọc nếu cần
                        .Select(pp => new PriceViewModel
                        {
                            Id = pp.Id,
                            Price = pp.Price,
                            PriceType = pp.PriceType,
                            StartDate = pp.StartDate,
                            EndDate = pp.EndDate
                        })
                        .ToList(),
                Images = product.ProductImages.Select(x => new ImageProductViewModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Caption = x.Caption
                }).ToList()
            };
            if (interaction != null)
                detailVM.IsInteractive = interaction.Type;
            return new ApiSuccessResult<ProductDetailViewModel>(detailVM);
        }
        public async Task<ApiResult<ProductDetailViewModel>> GetProductWithoutLogin(int itemId)
        {
            var product = await _db.ProductDetails
                .Include(x => x.Item)
                .ThenInclude(x => x.User)
                .Include(x => x.Category)
                .Include(x => x.ProductPrices)
                .Include(x => x.ProductImages)
                .FirstOrDefaultAsync(x => x.ItemId == itemId);
            if (product == null) return new ApiErrorResult<ProductDetailViewModel>("Không tìm thấy sản phẩm");
            var detailVM = new ProductDetailViewModel
            {
                Id = product.Item.Id,
                ProductId = product.Id,
                CategoryIcon = product.Category.SeoImage,
                Title = product.Item.Title,
                Detail = product.Detail,
                LikeCount = product.LikeCount,
                DisLikeCount = product.DislikeCount,
                DownloadCount = product.DownloadCount,
                Version = product.Version,
                Note = product.Note,
                IsOwned = false,
                CategoryName = product.Category.Name,
                SeoAlias = product.Item.SeoAlias,
                TimeCreates = product.Item.CreatedDate,
                TimeUpdates = product.Item.ModifiedDate,
                CategorySeoAlias = product.Category.SeoAlias,
                Description = product.Item.Description,
                Thumbnail = product.Item.Thumbnail,
                Username = product.Item.User.UserName!,
                Author = product.Item.User.FullName,
                ViewCount = product.Item.ViewCount,
                IsInteractive = DataAccess.Enums.InteractionType.None,
                Prices = product.ProductPrices
                        .Where(pp => !pp.ProductDetail.IsDeleted && pp.EndDate > DateTime.Now) // Lọc nếu cần
                        .Select(pp => new PriceViewModel
                        {
                            Id = pp.Id,
                            Price = pp.Price,
                            PriceType = pp.PriceType,
                            StartDate = pp.StartDate,
                            EndDate = pp.EndDate
                        })
                        .ToList(),
                Images = product.ProductImages.Select(x => new ImageProductViewModel
                {
                    Id = x.Id,
                    ImageUrl = x.ImageUrl,
                    Caption = x.Caption
                }).ToList()
            };
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
    }
}
