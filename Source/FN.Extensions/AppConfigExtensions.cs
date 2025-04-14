using CloudinaryDotNet;
using FirebaseAdmin;
using FN.Application.Helper.Images;
using FN.Application.Helper.Mail;
using FN.Utilities;
using FN.ViewModel.Helper;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IO.Compression;


namespace FN.Extensions
{
    public static class AppConfigExtensions
    {
        public static IApplicationBuilder ConfigureCORS(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseCors(options =>
                options
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowAnyOrigin()
                    .AllowCredentials()
                    .WithOrigins(
                    "http://127.0.0.1:5500",
                    "http://localhost:4200",
                    "https://mrkatsu.io.vn",
                    "https://katsudev.vercel.app",
                    "https://katsudev.netlify.app")
                );
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
        public static IServiceCollection ConfigureFirebase(this IServiceCollection services, IConfiguration config)
        {
            var firebaseSettings = config.GetSection(SystemConstant.FIREBASE_SETTINGS).Get<FirebaseSettings>();
            if (firebaseSettings == null) throw new System.Exception("Firebase settings not found");
            
            Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", @$"{firebaseSettings.CredentialFile}");
            FirebaseApp.Create(new AppOptions()
            {
                Credential = GoogleCredential.GetApplicationDefault(),
                ProjectId = firebaseSettings.ProjectId
            });
            services.AddSingleton(FirestoreDb.Create(firebaseSettings.ProjectId));
            return services;
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
            services.Configure<IISServerOptions>(options =>
            {
                options.MaxRequestBodySize = int.MaxValue;
            });

            services.Configure<KestrelServerOptions>(options =>
            {
                options.Limits.MaxRequestBodySize = int.MaxValue;
            });

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
