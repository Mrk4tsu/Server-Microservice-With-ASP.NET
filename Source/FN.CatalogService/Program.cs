using FN.Application.Catalog.Blogs;
using FN.Application.Catalog.Blogs.BlogComments;
using FN.Application.Catalog.Blogs.Interactions;
using FN.Application.Catalog.Categories;
using FN.Application.Catalog.Product;
using FN.Application.Catalog.Product.Interactions;
using FN.Application.Catalog.Product.Notifications;
using FN.Application.Catalog.Product.Pattern;
using FN.Application.Catalog.Product.Prices;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer(80);

builder.Services.AddControllers();
builder.Services.AddSwaggerExplorer()
    .InjectDbContext(builder.Configuration)
    .InjectRedis(builder.Configuration)
    .InjectMongoDb(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .AddIdentityAuth(builder.Configuration)
    .ConfigureIdentityOptions()
    .ConfigureServicePayload()
    .ConfigureFirebase(builder.Configuration)
    .AddImageConfig(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddScoped<INotifyService, NotifyService>();

builder.Services.AddScoped<IProductPublicService, ProductPublicService>();
builder.Services.AddScoped<IProductManageService, ProductManageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPriceProductService, PriceProductService>();
builder.Services.AddScoped<IBlogService, BlogService>();
builder.Services.AddScoped<IBlogCommentRepository, BlogCommentRepository>();

// Đăng ký các chiến lược sản phẩm
builder.Services.AddScoped<NewProductsStrategy>();
builder.Services.AddScoped<FeaturedProductsStrategy>();
builder.Services.AddScoped<RecommendProductsStrategy>();
builder.Services.AddScoped<ProductContext>();
builder.Services.AddScoped<IProductStrategyFactory, ProductStrategyFactory>();

// Đăng ký BlogInteraction như một dịch vụ scoped
builder.Services.AddScoped<BlogInteraction>();
builder.Services.AddScoped<ProductInteraction>();

// Đăng ký các trạng thái như các dịch vụ scoped
builder.Services.AddScoped<NoInteractionBlogState>();
builder.Services.AddScoped<LikedBlogState>();
builder.Services.AddScoped<DislikedBlogState>();

builder.Services.AddScoped<NoInteractionProductState>();
builder.Services.AddScoped<LikedProductState>();
builder.Services.AddScoped<DislikedProductState>();

// Đăng ký IInteractionState để inject vào BlogInteraction
builder.Services.AddScoped<IBlogInteractionState, NoInteractionBlogState>(); // Mặc định là NoInteractionState
builder.Services.AddScoped<IProductInteractionState, NoInteractionProductState>(); // Mặc định là NoInteractionState

builder.Logging.AddConsole();

var app = builder.Build();

app.ConfigureCORS(builder.Configuration)
    .ConfigureSwaggerExplorer()
    .ConfigureAppForwarded()
    .ConfigureAppPayLoad()
    .AddIdentityAuthMiddlewares();

app.UseWebSockets();

app.MapControllers();

app.MapHub<NotifyHub>("/notify").RequireAuthorization();


app.Run();
