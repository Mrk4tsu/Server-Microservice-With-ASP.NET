using FN.Application.Catalog.Product.Notifications;
using FN.Application.Systems.Orders;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer(80);

builder.Services.AddSwaggerExplorer()
        .InjectDbContextPool(builder.Configuration)
        .AddIdentityHandlersAndStores()
        .AddIdentityAuth(builder.Configuration)
        .ConfigureIdentityOptions();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureCORS(app.Configuration)
    .ConfigureSwaggerExplorer()
    .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
