using FN.AIService.Services;
using FN.AIService.Services.Chats;
using FN.Extensions;

var builder = WebApplication.CreateBuilder(args);

string _apiKey = builder.Configuration["GeminiAPIKey"]!;

builder.Services.ConfigureDbContext(builder.Configuration)
                .AddIdentityAuth(builder.Configuration)
                .ConfigureMongoDb(builder.Configuration)
                .ConfigureServicePayload()
                .AddSwaggerExplorer()
                .ConfigureIdentityOptions();

builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<GeminiService>();
builder.Services.AddScoped<IGeminiProductAnalysisService, GeminiProductAnalysisService>();
builder.Services.AddScoped<IChatSessionRepository, ChatSessionRepository>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddHttpClient();

var app = builder.Build();

//app.UseCors(option => option
//            .AllowAnyOrigin()
//            .AllowAnyMethod()
//            .AllowAnyHeader()
//            .WithExposedHeaders("Content-Dispostion"));

app.ConfigureCORS(builder.Configuration)
    .ConfigureSwaggerExplorer()
   .AddIdentityAuthMiddlewares();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
