using AutoMapper;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.ViewModel.Catalog.Blogs;
using FN.ViewModel.Catalog.Categories;
using FN.ViewModel.Catalog.Products;
using FN.ViewModel.Catalog.Products.FeedbackProduct;
using FN.ViewModel.Systems.User;

namespace FN.Application.Base
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<AppUser, UserViewModel>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Id));

            CreateMap<Blog, BlogViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Item.Id))
            .ForMember(dest => dest.BlogId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Item.User.FullName))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item.Description))
            .ForMember(dest => dest.SeoAlias, opt => opt.MapFrom(src => src.Item.SeoAlias))
            .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => src.Item.Thumbnail))
            .ForMember(dest => dest.TimeCreate, opt => opt.MapFrom(src => src.Item.CreatedDate))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Item.Title))
            .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.Item.ViewCount));

            CreateMap<Blog, BlogDetailViewModel>()
                .IncludeBase<Blog, BlogViewModel>()
                .ForMember(dest => dest.SeoTitle, opt => opt.MapFrom(src => src.Item.SeoTitle))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.LikeCount))
                .ForMember(dest => dest.DislikeCount, opt => opt.MapFrom(src => src.DislikeCount))
                .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Detail))
                .ForMember(dest => dest.TimeUpdate, opt => opt.MapFrom(src => src.Item.ModifiedDate))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Item.User.UserName ?? "Unknown"));
            #region [Product]
            // ProductDetail to ProductViewModel
            CreateMap<ProductDetail, ProductViewModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Item.Id))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.Item.UserId))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Item.Title))
                .ForMember(dest => dest.NormalizeTitle, opt => opt.MapFrom(src => src.Item.NormalizedTitle))
                .ForMember(dest => dest.Thumbnail, opt => opt.MapFrom(src => src.Item.Thumbnail))
                .ForMember(dest => dest.SeoAlias, opt => opt.MapFrom(src => src.Item.SeoAlias))
                .ForMember(dest => dest.Username, opt => opt.MapFrom(src => src.Item.User.UserName))
                .ForMember(dest => dest.Version, opt => opt.MapFrom(src => src.Version))
                .ForMember(dest => dest.DownloadCount, opt => opt.MapFrom(src => src.DownloadCount))
                .ForMember(dest => dest.CategoryIcon, opt => opt.MapFrom(src => src.Category.SeoImage))
                .ForMember(dest => dest.CategorySeoAlias, opt => opt.MapFrom(src => src.Category.SeoAlias))
                .ForMember(dest => dest.TimeCreates, opt => opt.MapFrom(src => src.Item.CreatedDate))
                .ForMember(dest => dest.TimeUpdates, opt => opt.MapFrom(src => src.Item.ModifiedDate))
                .ForMember(dest => dest.Prices, opt => opt.MapFrom(src => src.ProductPrices
                    .Where(pp => !src.IsDeleted && pp.EndDate > DateTime.Now)));
            // ProductDetail to ProductDetailViewModel (kế thừa từ ProductViewModel)
            CreateMap<ProductDetail, ProductDetailViewModel>()
                .IncludeBase<ProductDetail, ProductViewModel>()
                .ForMember(dest => dest.Detail, opt => opt.MapFrom(src => src.Detail))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.ViewCount, opt => opt.MapFrom(src => src.Item.ViewCount))
                .ForMember(dest => dest.LikeCount, opt => opt.MapFrom(src => src.LikeCount))
                .ForMember(dest => dest.DisLikeCount, opt => opt.MapFrom(src => src.DislikeCount))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Item.Description))
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category.Name))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Item.User.FullName))
                .ForMember(dest => dest.Images, opt => opt.MapFrom(src => src.ProductImages));
            // ProductPrice to PriceViewModel
            CreateMap<ProductPrice, PriceViewModel>();
            // ProductImage to ImageProductViewModel
            CreateMap<ProductImage, ImageProductViewModel>();
            // FeedBack to FeedbackViewModel
            CreateMap<FeedBack, FeedbackViewModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.User.FullName))
            .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.Content))
            .ForMember(dest => dest.Rate, opt => opt.MapFrom(src => src.Rate))
            .ForMember(dest => dest.TimeCreated, opt => opt.MapFrom(src => src.TimeCreated))
            .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.User.Avatar));
            #endregion
        }
    }
}
