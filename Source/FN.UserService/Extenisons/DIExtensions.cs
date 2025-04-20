using FN.Application.Catalog.Product.Notifications;
using FN.Application.Helper.Devices;
using FN.Application.Systems.Token;
using FN.Application.Systems.User;

namespace FN.UserService.Extenisons
{
    public static class DIExtensions
    {
        public static IServiceCollection AddUserService(this IServiceCollection services)
        {
            services.AddScoped<INotifyService, NotifyService>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IUserService, FN.Application.Systems.User.UserService>();
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IDeviceService, DeviceService>();
            return services;
        }
    }
}
