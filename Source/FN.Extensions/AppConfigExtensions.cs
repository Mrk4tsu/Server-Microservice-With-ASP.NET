using CloudinaryDotNet;
using FN.Application.Helper.Images;
using FN.Application.Helper.Mail;
using FN.Utilities;
using FN.ViewModel.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ocelot.Values;
using System.IO.Compression;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace FN.Extensions
{
    public static class AppConfigExtensions
    {
        public static IApplicationBuilder ConfigureCORS(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseCors(options =>
            options.WithOrigins(
                "http://localhost:4200",
                "https://mrkatsu.io.vn",
                "https://katsudev.vercel.app",
                "https://katsudev.netlify.app")
            .AllowAnyMethod()
            .AllowAnyHeader());
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            return app;
        }
        public static IApplicationBuilder ConfigureAppForwarded(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders();
            return app;
        }
        public static IApplicationBuilder ConfigureWebSocket(this IApplicationBuilder app, string path)
        {
            app.UseWebSockets();
            app.Use(async (context, next) =>
            {
                Console.WriteLine($"WebSocket request to: {context.Request.Path}");
                if (context.Request.Path == path && context.WebSockets.IsWebSocketRequest)
                {
                    Console.WriteLine("WebSocket request accepted.");
                    using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
                    await HandleOrderWebSocket(webSocket);
                }
                else
                {
                    Console.WriteLine("WebSocket request failed.");
                    context.Response.StatusCode = 400;
                    await next();
                }
            });
            return app;
        }
        public static IServiceCollection ConfigureServiceForwarded(this IServiceCollection services)
        {
            services.Configure<ForwardedHeadersOptions>(options =>
            {
                options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
                options.KnownNetworks.Clear();
                options.KnownProxies.Clear();
            });
            return services;
        }
        public static IServiceCollection AddSmtpConfig(this IServiceCollection services, IConfiguration config)
        {
            services.Configure<MailSetting>(config.GetSection(SystemConstant.SMTP_SETTINGS));
            services.AddSingleton<IMailService, MailService>();
            return services;
        }
        public static IServiceCollection AddImageConfig(this IServiceCollection services, IConfiguration config)
        {
            var cloudinarySettings = config.GetSection(SystemConstant.CLOUDINARY_SETTINGS).Get<CloudinarySettings>();
            services.AddSingleton(new Cloudinary(new Account(
                cloudinarySettings!.CloudName,
                cloudinarySettings.ApiKey,
                cloudinarySettings.ApiSecret
            )));

            services.AddSingleton<IImageService, ImageService>();
            return services;
        }
        public static IServiceCollection ConfigureServicePayload(this IServiceCollection services)
        {
            services.AddResponseCompression(options =>
            {
                options.EnableForHttps = true; // Cho phép nén trên HTTPS
                options.Providers.Add<GzipCompressionProvider>();
                options.Providers.Add<BrotliCompressionProvider>();
            });
            services.Configure<BrotliCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Fastest; // Có thể là Optimal, Fastest, NoCompression
            });
            services.Configure<GzipCompressionProviderOptions>(options =>
            {
                options.Level = CompressionLevel.Optimal;
            });
            return services;
        }
        public static IApplicationBuilder ConfigureAppPayLoad(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Headers["Content-Encoding"] == "gzip")
                {
                    context.Request.Body = new GZipStream(context.Request.Body, CompressionMode.Decompress);
                }
                await next();
            });

            app.UseResponseCompression();
            return app;
        }

        private static async Task HandleOrderWebSocket(WebSocket webSocket)
        {
            var buffer = new byte[1024 * 4];
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            while (!result.CloseStatus.HasValue)
            {
                // Giả lập xử lý order
                var orderData = Encoding.UTF8.GetString(buffer, 0, result.Count);
                var orderId = Guid.NewGuid().ToString();
                var response = new { OrderId = orderId, Total = 20 }; // Payload nhỏ gọn
                var responseBytes = Encoding.UTF8.GetBytes(JsonSerializer.Serialize(response));

                await webSocket.SendAsync(new ArraySegment<byte>(responseBytes), result.MessageType, true, CancellationToken.None);
                result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            }

            await webSocket.CloseAsync(result.CloseStatus.Value, result.CloseStatusDescription, CancellationToken.None);
        }
    }
}
