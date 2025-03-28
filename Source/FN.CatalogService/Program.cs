using FN.Application.Catalog.Blogs;
using FN.Application.Catalog.Blogs.Interactions;
using FN.Application.Catalog.Categories;
using FN.Application.Catalog.Product;
using FN.Application.Catalog.Product.Prices;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerExplorer()
    .InjectDbContext(builder.Configuration)
    .InjectRedis(builder.Configuration)
    .InjectMongoDb(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .AddIdentityAuth(builder.Configuration)
    .ConfigureIdentityOptions()
    .ConfigureServicePayload()
    .AddImageConfig(builder.Configuration);

builder.Services.AddScoped<IProductPublicService, ProductPublicService>();
builder.Services.AddScoped<IProductManageService, ProductManageService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IPriceProductService, PriceProductService>();
builder.Services.AddScoped<IBlogService, BlogService>();

// Đăng ký BlogInteraction như một dịch vụ scoped
builder.Services.AddScoped<BlogInteraction>();

// Đăng ký các trạng thái như các dịch vụ scoped
builder.Services.AddScoped<NoInteractionState>();
builder.Services.AddScoped<LikedState>();
builder.Services.AddScoped<DislikedState>();

// Đăng ký IInteractionState để inject vào BlogInteraction
builder.Services.AddScoped<IInteractionState, NoInteractionState>(); // Mặc định là NoInteractionState

var app = builder.Build();

app.ConfigureSwaggerExplorer()
    .ConfigureCORS(builder.Configuration)
    .ConfigureAppForwarded()
    .ConfigureAppPayLoad()
    .AddIdentityAuthMiddlewares();

app.MapControllers();

app.Run();
