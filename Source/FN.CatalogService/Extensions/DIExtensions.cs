using FN.Application.Catalog.Blogs.BlogComments;
using FN.Application.Catalog.Blogs;
using FN.Application.Catalog.Blogs.Interactions;
using FN.Application.Catalog.Categories;
using FN.Application.Catalog.Product;
using FN.Application.Catalog.Product.Interactions;
using FN.Application.Catalog.Product.Pattern;
using FN.Application.Catalog.Product.Prices;
using FN.Application.Catalog.Product.Notifications;
using FN.Application.Systems.Events;

namespace FN.CatalogService.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddAdditionalProductServices(this IServiceCollection services)
        {
            services.AddScoped<INotifyService, NotifyService>();
            services.AddScoped<ISaleEventService, SaleEventService>();
            return services;
        }
        public static IServiceCollection AddProductStrategyServices(this IServiceCollection services)
        {
            // Đăng ký các chiến lược sản phẩm
            services.AddScoped<NewProductsStrategy>();
            services.AddScoped<FeaturedProductsStrategy>();
            services.AddScoped<RecommendProductsStrategy>();
            services.AddScoped<ProductContext>();
            services.AddScoped<IProductStrategyFactory, ProductStrategyFactory>();
            return services;
        }
        public static IServiceCollection AddProductInteractionServices(this IServiceCollection services)
        {
            services.AddScoped<ProductInteraction>();

            services.AddScoped<NoInteractionProductState>();
            services.AddScoped<LikedProductState>();
            services.AddScoped<DislikedProductState>();
            services.AddScoped<IProductInteractionState, NoInteractionProductState>(); // Mặc định là NoInteractionState

            return services;
        }
        public static IServiceCollection AddBlogInteractionServices(this IServiceCollection services)
        {
            // Đăng ký BlogInteraction như một dịch vụ scoped
            services.AddScoped<BlogInteraction>();

            // Đăng ký các trạng thái như các dịch vụ scoped
            services.AddScoped<NoInteractionBlogState>();
            services.AddScoped<LikedBlogState>();
            services.AddScoped<DislikedBlogState>();

            // Đăng ký IInteractionState để inject vào BlogInteraction
            services.AddScoped<IBlogInteractionState, NoInteractionBlogState>(); // Mặc định là NoInteractionState

            return services;
        }
        public static IServiceCollection AddMainProductServices(this IServiceCollection services)
        {
            services.AddScoped<IProductPublicService, ProductPublicService>();
            services.AddScoped<IProductManageService, ProductManageService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IPriceProductService, PriceProductService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();
            return services;
        }
    }
}
