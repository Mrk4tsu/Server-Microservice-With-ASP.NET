using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.Utilities;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using StackExchange.Redis;
using System.Data;

namespace FN.Forum.Extensions
{
    public static class CustomExtension
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString(SystemConstant.DB_CONNECTION_STRING);

            services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddSingleton<IDbConnection>(sp => new MySqlConnection(connectionString));
            return services;
        }
        public static IServiceCollection ConfigureRedis(this IServiceCollection services, IConfiguration config)
        {
            var connectionStringRedis = config.GetConnectionString(SystemConstant.REDIS_CONNECTION_STRING);
            if (string.IsNullOrEmpty(connectionStringRedis))
                throw new ArgumentNullException(nameof(connectionStringRedis), "Redis connection string is missing.");
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = connectionStringRedis;
            });
            services.AddSingleton<IConnectionMultiplexer>(sp =>
            {
                return ConnectionMultiplexer.Connect(connectionStringRedis);
            });
            services.AddSingleton(sp =>
            {
                var multiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
                return multiplexer.GetDatabase();
            });

            services.AddSingleton<IRedisService, RedisService>();
            return services;
        }
        public static IApplicationBuilder ConfigureCORS(this IApplicationBuilder app, IConfiguration config)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });
            app.UseCors(options =>
                options
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithOrigins(
                        "https://mrkatsu.io.vn",
                        "http://localhost:4200",
                        "https://katsudev.onrender.com",
                        "https://mrk4tsu.azurewebsites.net",
                        "https://katsudev.vercel.app"));


            return app;
        }

    }
}
