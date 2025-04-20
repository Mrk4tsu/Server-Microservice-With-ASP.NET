using FN.Application.Helper.Devices;
using FN.Application.Systems.Token;

namespace FN.EmailService.Extensions
{
    public static class DIExtensions
    {
        public static IServiceCollection AddNotificationServices(this IServiceCollection services)
        {
            services.AddScoped<MailSubscriber>();

            services.AddScoped<IDeviceService, DeviceService>();

            services.AddHostedService<MailSubscriber>();

            services.AddScoped<ITokenService, TokenService>();

            services.AddControllers();
            return services;
        }
    }
}
