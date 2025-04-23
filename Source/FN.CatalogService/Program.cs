using FN.Application.Catalog.Product.Notifications;
using FN.Application.Systems.Events;
using FN.CatalogService.Extensions;
using FN.CatalogService.Kafka;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer(80);

builder.Services.AddHttpClient();

builder.Services.AddControllers();

builder.Services.AddSwaggerExplorer()
    .ConfigureDbContext(builder.Configuration)
    .ConfigureRedis(builder.Configuration)
    .ConfigureMongoDb(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .AddIdentityAuth(builder.Configuration)
    .ConfigureIdentityOptions()
    .ConfigureServicePayload()
    .ConfigureFirebase(builder.Configuration)
    .ConfigureHangFireServices(builder.Configuration)
    .AddImageConfig(builder.Configuration);

builder.Services.AddSignalR();

builder.Services.AddScoped<ISaleEventService, SaleEventService>();


builder.Services
    .AddMainProductServices()
    .AddProductInteractionServices()
    .AddBlogInteractionServices()
    .AddProductStrategyServices()
    .AddAdditionalProductServices();

builder.Services.AddHostedService<KafkaConsumer>();

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

await app.ConfigureAppHangfire();

app.Run();
