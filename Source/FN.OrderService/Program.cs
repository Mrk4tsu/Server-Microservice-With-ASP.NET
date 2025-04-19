using FN.Application.Systems.Events;
using FN.Application.Systems.Kafka;
using FN.Application.Systems.Orders;
using FN.Extensions;
using FN.Utilities;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer(80);

builder.Services.AddSwaggerExplorer()
        .ConfigureDbContext(builder.Configuration)
        .AddIdentityHandlersAndStores()
        .ConfigureRedis(builder.Configuration)
        .AddIdentityAuth(builder.Configuration)
        .ConfigureHangFireServices(builder.Configuration)
        .ConfigureServicePayload()
        .ConfigureIdentityOptions();

builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<ISaleEventService, SaleEventService>();
builder.Services.AddScoped<IKafkaProducer>(provider =>new KafkaProducer(SystemConstant.EVENT_PAYMENT_GROUP_KAFKA));

builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureCORS(app.Configuration)
    .ConfigureSwaggerExplorer()
    .ConfigureAppPayLoad()
    .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
