using FN.Application.Base;
using FN.DataAccess;
using FN.Utilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using System.Data;

namespace FN.Extensions
{
    public static class EFCoreExtensions
    {
        public static IServiceCollection ConfigureDbContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString(SystemConstant.DB_CONNECTION_STRING);

            services.AddDbContext<AppDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

            services.AddSingleton<IDbConnection>(sp => new MySqlConnection(connectionString));
            services.AddAutoMapper(typeof(AutoMapperProfile));
            return services;
        }
        public static IServiceCollection ConfigureDbFactoryContext(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString(SystemConstant.DB_CONNECTION_STRING);

            services.AddDbContextFactory<AppDbContext>(options =>
                options.UseMySql(connectionString,
                ServerVersion.AutoDetect(connectionString)),
            lifetime: ServiceLifetime.Scoped);

            services.AddSingleton<IDbConnection>(sp => new MySqlConnection(connectionString));
            services.AddAutoMapper(typeof(AutoMapperProfile));
            return services;
        }
        public static IServiceCollection InjectDbContextPool(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString(SystemConstant.DB_CONNECTION_STRING);

            // Đăng ký DbContext với Pooling mặc định của MySQL
            services.AddDbContextPool<AppDbContext>(options =>
            {
                options.UseMySql(
                    connectionString,
                    ServerVersion.AutoDetect(connectionString),
                    mySqlOptions =>
                    {
                        mySqlOptions.EnableRetryOnFailure(
                            maxRetryCount: 5,
                            maxRetryDelay: TimeSpan.FromSeconds(10),
                            errorNumbersToAdd: null);
                    });
            });

            services.AddAutoMapper(typeof(AutoMapperProfile));

            return services;
        }
    }
}
