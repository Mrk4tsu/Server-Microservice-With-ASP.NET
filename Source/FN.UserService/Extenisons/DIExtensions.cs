using FN.Application.Catalog.Product.Notifications;
using FN.Application.Helper.Devices;
using FN.Application.Systems.Token;
using FN.Application.Systems.User;
using Microsoft.AspNetCore.Identity;

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
        public static IServiceCollection AddOAuth(this IServiceCollection services, IConfiguration config)
        {
            services.AddAuthentication()
                    .AddGoogle(options =>
                    {
                        options.ClientId = config["Authentication:Google:ClientId"]!;
                        options.ClientSecret = config["Authentication:Google:ClientSecret"]!;
                        options.SignInScheme = IdentityConstants.ExternalScheme;
                        options.CallbackPath = "/signin-google";
                        // Lấy thêm thông tin từ Google
                        options.Scope.Add("profile");
                        options.Scope.Add("email");
                    });
            return services;
        }
    }
}
