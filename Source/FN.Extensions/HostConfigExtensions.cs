using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace FN.Extensions
{
    public static class HostConfigExtensions
    {
        public static IWebHostBuilder ConfigureKestrelServer(this IWebHostBuilder webHostBuilder, int port = 80)
        {
            webHostBuilder.ConfigureServices((context, services) =>
            {
                //var env = context.HostingEnvironment;
                //if (!env.IsDevelopment())
                //{
                //    webHostBuilder.ConfigureKestrel(options =>
                //    {
                //        options.ListenAnyIP(port);
                //    });
                //}
            });

            return webHostBuilder;
            //return webHostBuilder.ConfigureKestrel(options =>
            //{
            //    options.ListenAnyIP(port);
            //});
        }
    }
}
