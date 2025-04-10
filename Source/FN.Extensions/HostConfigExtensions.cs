using Microsoft.AspNetCore.Hosting;

namespace FN.Extensions
{
    public static class HostConfigExtensions
    {
        public static IWebHostBuilder ConfigureKestrelServer(this IWebHostBuilder webHostBuilder, int port = 80)
        {
            return webHostBuilder.ConfigureKestrel(options =>
            {
                options.ListenAnyIP(port);
            });
        }
    }
}
