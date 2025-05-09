using FN.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.ConfigureKestrelServer();

var evn = builder.Environment;
Console.WriteLine(evn.EnvironmentName);
builder.Configuration.AddJsonFile($"ocelot.{evn.EnvironmentName}.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);

builder.Services.AddControllers();
builder.Services.ConfigureServicePayload()
    .AddIdentityAuth(builder.Configuration)
    .ConfigureServiceForwarded();

builder.Services.AddSignalR();

var app = builder.Build();

app.ConfigureCORS(builder.Configuration)
    .ConfigureAppForwarded()
    .ConfigureAppPayLoad();

app.UseWebSockets();

app.UseAuthorization();

app.MapControllers();

await app.UseOcelot();

app.Run();
