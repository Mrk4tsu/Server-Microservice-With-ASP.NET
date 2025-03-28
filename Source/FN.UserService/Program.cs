using FN.Application.Helper.Devices;
using FN.Application.Systems.Token;
using FN.Application.Systems.User;
using FN.Extensions;
using System.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSwaggerExplorer()
    .InjectDbContextPool(builder.Configuration)
    .InjectRedis(builder.Configuration)
    .InjectMongoDb(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .AddIdentityAuth(builder.Configuration)
    .ConfigureIdentityOptions()
    .ConfigureServicePayload()
    .AddImageConfig(builder.Configuration);

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IDeviceService, DeviceService>();

var app = builder.Build();

app.ConfigureSwaggerExplorer()
    .ConfigureCORS(builder.Configuration)
    .ConfigureAppForwarded()
    .AddIdentityAuthMiddlewares()
    .ConfigureAppPayLoad();

app.MapControllers();

app.Run();
