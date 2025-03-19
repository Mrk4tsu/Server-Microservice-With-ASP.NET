﻿using AutoMapper;
using AutoMapper.QueryableExtensions;
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
        public async Task<ApiResult<ProductDetailViewModel>> GetProduct(int itemId)
        {
            var product = await _db.ProductDetails
                .Where(x => x.ItemId == itemId)
                .ProjectTo<ProductDetailViewModel>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

            if (product == null)
                return new ApiErrorResult<ProductDetailViewModel>("Không tìm thấy sản phẩm");

            return new ApiSuccessResult<ProductDetailViewModel>(product);
        }
        public async Task<ApiResult<PagedResult<ProductViewModel>>> GetProducts(ProductPagingRequest request)
        {
            var facade = new GetProductFacade(_db, _dbRedis!, null!, _mapper);
            return await facade.GetProducts(request, false, false, null);
        }
    }
}
