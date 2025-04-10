using FN.Application.Helper.Devices;
using FN.Application.Systems.Token;
using FN.EmailService;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer(80);

builder.Services
    .InjectDbContext(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .InjectRedis(builder.Configuration)
    .InjectMongoDb(builder.Configuration)
    .AddSmtpConfig(builder.Configuration);

builder.Services.AddScoped<MailSubscriber>();

builder.Services.AddScoped<IDeviceService, DeviceService>();

builder.Services.AddHostedService<MailSubscriber>();

builder.Services.AddScoped<ITokenService, TokenService>();

builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureCORS(builder.Configuration);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
