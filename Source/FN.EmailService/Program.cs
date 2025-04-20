using FN.Application.Helper.Devices;
using FN.Application.Systems.Token;
using FN.EmailService;
using FN.EmailService.Extensions;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer(80);

builder.Services
    .ConfigureDbContext(builder.Configuration)
    .AddIdentityHandlersAndStores()
    .ConfigureRedis(builder.Configuration)
    .ConfigureMongoDb(builder.Configuration)
    .AddSmtpConfig(builder.Configuration);

builder.Services.AddNotificationServices();

builder.Logging.AddConsole();

var app = builder.Build();

app.ConfigureCORS(builder.Configuration);

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
