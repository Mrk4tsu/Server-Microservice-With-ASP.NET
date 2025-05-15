using FN.Forum.Services;

namespace FN.Forum.Extensions
{
    public static class ServiceExtension
    {
        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<ITopicService, TopicServices>();
            services.AddScoped<IReplyService, ReplyServices>();
            return services;
        }
    }
}
