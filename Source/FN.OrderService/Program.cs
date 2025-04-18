using FN.Application.Catalog.Product.Notifications;
using FN.Application.Systems.Events;
using FN.Application.Systems.Orders;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer(80);

builder.Services.AddSwaggerExplorer()
        .InjectDbContext(builder.Configuration)
        .AddIdentityHandlersAndStores()
        .InjectRedis(builder.Configuration)
        .AddIdentityAuth(builder.Configuration)
        .ConfigureServicePayload()
        .ConfigureHangFireServices(builder.Configuration)
        .ConfigureIdentityOptions();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISaleEventService, SaleEventService>();

builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureCORS(app.Configuration)
    .ConfigureSwaggerExplorer()
    .ConfigureAppPayLoad()
    .AddIdentityAuthMiddlewares();

await app.ConfigureAppHangfire();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
