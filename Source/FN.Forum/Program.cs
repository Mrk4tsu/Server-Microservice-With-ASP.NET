using FN.Extensions;
using FN.Forum.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureDbContext(builder.Configuration)
                .ConfigureRedis(builder.Configuration)
                .AddIdentityHandlersAndStores()
                .ConfigureIdentityOptions()
                .AddIdentityAuth(builder.Configuration)
                .AddSwaggerExplorer();
builder.Services.AddScoped<ITopicService, TopicServices>();
builder.Services.AddScoped<IReplyService, ReplyServices>();

builder.Services.AddControllers();

var app = builder.Build();

app.ConfigureCORS(builder.Configuration)
   .ConfigureSwaggerExplorer()
   .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
