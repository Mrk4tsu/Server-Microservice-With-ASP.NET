using FN.Forum.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext(builder.Configuration)
                .ConfigureRedis(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration)
                .AddSwaggerExplorer()
                .AddServices();

builder.Services.AddControllers();



var app = builder.Build();

app.ConfigureCORS(builder.Configuration)
   .ConfigureSwaggerExplorer()
   .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
