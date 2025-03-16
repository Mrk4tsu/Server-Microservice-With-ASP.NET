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
using Microsoft.Extensions.Hosting;
using Ocelot.Middleware;
using Ocelot.Values;
using System.IO.Compression;

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
        public static IApplicationBuilder ConfigureAppExplorer(this IApplicationBuilder app)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            return app;
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
    }
}
