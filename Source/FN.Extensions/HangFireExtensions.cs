using FN.Application.Systems.Events;
using FN.Utilities;
using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace FN.Extensions
{
    public static class HangFireExtensions
    {
        public static IServiceCollection ConfigureHangFireServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionStringRedis = config.GetConnectionString(SystemConstant.REDIS_CONNECTION_STRING);
            services.AddHangfire(config => config.UseRedisStorage(ConnectionMultiplexer.Connect(connectionStringRedis!)));

            services.AddHangfireServer();

            services.AddScoped<ISeasonalEventScheduler, SeasonalEventScheduler>();

            return services;
        }
        public async static Task<IApplicationBuilder> ConfigureAppHangfire(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var scheduler = scope.ServiceProvider.GetRequiredService<ISeasonalEventScheduler>();
                await scheduler.ScheduleSeasonalEvents();


                // Kiểm tra và kích hoạt sự kiện hiện tại nếu cần
                var currentEvent = await scheduler.GetCurrentSeasonalEvent();
                if (currentEvent == null)
                {
                    DateTime now = new TimeHelper.Builder()
                    .SetTimestamp(DateTime.UtcNow)
                    .SetTimeZone("SE Asia Standard Time")
                    .SetRemoveTick(true).Build();

                    var upcomingEvent = await scheduler.GetUpcomingSeasonalEvent();

                    if (upcomingEvent != null)
                    {
                        var timeUntilEvent = upcomingEvent.StartDate - now;
                        // Kích hoạt sự kiện hiện tại
                        scheduler.Run(upcomingEvent, timeUntilEvent);
                        
                    }
                }
            }
            return app;
        }
    }
}
