using FN.Application.Systems.Orders;
using FN.Extensions;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

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
    .ConfigureCORS(app.Configuration)
    .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
