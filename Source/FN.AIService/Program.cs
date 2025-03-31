using FN.Application.Catalog.Blogs;
using FN.Extensions;
using GeminiAIDev.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddScoped<IImageSearchService, ImageSearchService>();
string _apiKey = builder.Configuration["GeminiAPIKey"]!;
//builder.Services.AddSingleton(new GeminiApiClient(apiKey));
builder.Services.AddSingleton<GameTitleExtractor>();
builder.Services.AddScoped<GeminiApiClient>(provide => new GeminiApiClient(
    provide.GetRequiredService<GameTitleExtractor>(),
    provide.GetRequiredService<IImageSearchService>(),
    _apiKey
    ));
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerExplorer();

var app = builder.Build();

app.UseCors(option => option
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader()
            .WithExposedHeaders("Content-Dispostion"));

app.ConfigureSwaggerExplorer();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
