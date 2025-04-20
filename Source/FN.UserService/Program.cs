using FN.Extensions;
using FN.UserService.Extenisons;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer();

builder.Services.AddControllers();
builder.Services.AddSwaggerExplorer()
    .InjectDbContextPool(builder.Configuration)
    .ConfigureServiceForwarded()
    .ConfigureRedis(builder.Configuration)
    .ConfigureMongoDb(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .AddIdentityAuth(builder.Configuration)
    .ConfigureIdentityOptions()
    .ConfigureServicePayload()
    .AddImageConfig(builder.Configuration);

builder.Services.AddUserService();

builder.Logging.AddConsole();

var app = builder.Build();

app.ConfigureCORS(builder.Configuration)
   .ConfigureAppForwarded()
   .ConfigureSwaggerExplorer()
   .AddIdentityAuthMiddlewares()
   .ConfigureAppPayLoad();

app.MapControllers();

app.Run();
