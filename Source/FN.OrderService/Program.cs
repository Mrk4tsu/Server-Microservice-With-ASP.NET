using FN.Application.Systems.Orders;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwaggerExplorer()
        .InjectDbContextPool(builder.Configuration)
        .AddIdentityHandlersAndStores()
        .AddIdentityAuth(builder.Configuration)
        .ConfigureIdentityOptions();

builder.Services.AddScoped<IOrderService, OrderService>();

builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureSwaggerExplorer()
    .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
