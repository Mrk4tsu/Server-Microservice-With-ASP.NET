using FN.Application.Systems.Redis;
using FN.DataAccess;
using FN.DataAccess.Entities;
using FN.DataAccess.Enums;
using FN.ViewModel.Systems.Events;
using Hangfire;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace FN.Application.Systems.Events
{
    public class SeasonalEventScheduler : ISeasonalEventScheduler
    {
        private readonly AppDbContext _db;
        private readonly IBackgroundJobClient _backgroundJob;
        private readonly IRedisService _redisService;
        private readonly DateTime _now;
        public SeasonalEventScheduler(AppDbContext db,
            IRedisService redisService,
            IBackgroundJobClient backgroundJob)
        {
            _db = db;
            _redisService = redisService;
            _backgroundJob = backgroundJob;
            _now = new TimeHelper.Builder()
               .SetTimestamp(DateTime.UtcNow)
               .SetTimeZone("SE Asia Standard Time")
               .SetRemoveTick(true).Build();
        }
        public async Task ScheduleSeasonalEvents()
        {
            var currentYear = _now.Year;
            var scheduledKey = $"seasonal_events_scheduled:{currentYear}";
            if (await _redisService.KeyExist(scheduledKey))
                return;
            // Lịch trình các sự kiện theo mùa
            var springEvent = await CreateSeasonalEvent(
                "Spring Sale",
                SeasonType.Spring,
                new DateTime(currentYear, 3, 20),
                new DateTime(currentYear, 6, 20));

            var summerEvent = await CreateSeasonalEvent(
                "Summer Sale",
                SeasonType.Summer,
                new DateTime(currentYear, 6, 21),
                new DateTime(currentYear, 9, 22));

            var autumnEvent = await CreateSeasonalEvent(
                "Autumn Sale",
                SeasonType.Autumn,
                new DateTime(currentYear, 9, 23),
                new DateTime(currentYear, 12, 20));

            var winterEvent = await CreateSeasonalEvent(
               "Winter Sale",
               SeasonType.Winter,
               new DateTime(currentYear, 12, 21),
               new DateTime(currentYear + 1, 3, 19));

            //Spring
            _backgroundJob.Schedule<ISaleEventService>(
               x => x.ActivateEvent(springEvent.Id),
               springEvent.StartDate);
            _backgroundJob.Schedule<ISaleEventService>(
            x => x.DeactivateEvent(springEvent.Id),
            springEvent.EndDate);

            //Summer
            _backgroundJob.Schedule<ISaleEventService>(
               x => x.ActivateEvent(summerEvent.Id),
               summerEvent.StartDate);
            _backgroundJob.Schedule<ISaleEventService>(
            x => x.DeactivateEvent(summerEvent.Id),
            summerEvent.EndDate);

            //Autumn
            _backgroundJob.Schedule<ISaleEventService>(
               x => x.ActivateEvent(autumnEvent.Id),
               autumnEvent.StartDate);
            _backgroundJob.Schedule<ISaleEventService>(
            x => x.DeactivateEvent(autumnEvent.Id),
            autumnEvent.EndDate);

            //Winter
            _backgroundJob.Schedule<ISaleEventService>(
               x => x.ActivateEvent(winterEvent.Id),
               winterEvent.StartDate);
            _backgroundJob.Schedule<ISaleEventService>(
               x => x.DeactivateEvent(winterEvent.Id),
               winterEvent.EndDate);

            await _redisService.SetValue(scheduledKey, "true", TimeSpan.FromDays(366));
        }
        public async Task<SaleEvent> GetCurrentSeasonalEvent()
        {
            var cacheKey = "current_seasonal_event";
            if (await _redisService.KeyExist(cacheKey))
            {
                var cachedEvent = await _redisService.GetValue<SaleEvent>(cacheKey);
                if (cachedEvent != null)
                {
                    return cachedEvent;
                }
            }
            var currentEvent = await _db.SaleEvents.FirstOrDefaultAsync(e => e.StartDate <= _now && e.EndDate >= _now && e.IsActive);
            if (currentEvent != null)
            {
                var cacheExpiry = currentEvent.EndDate - _now;
                await _redisService.SetValue(cacheKey, currentEvent, cacheExpiry);
            }
            return currentEvent;
        }
        public async Task<SaleEvent> GetUpcomingSeasonalEvent()
        {
            var upcomingEvent = await _db.SaleEvents
                .Where(e => e.StartDate > _now)
                .OrderBy(e => e.StartDate)
                .FirstOrDefaultAsync();
            return upcomingEvent;
        }
        private async Task<SaleEvent> CreateSeasonalEvent(string name,SeasonType season,DateTime startDate,DateTime endDate)
        {
            var seasonalEvent = new SaleEvent
            {
                Name = name,
                Season = season,
                Year = startDate.Year,
                StartDate = startDate,
                EndDate = endDate,
                IsActive = false,
                BannerImage = GetSeasonBanner(season),
                Description = $"Welcome to the {name} event! Enjoy exclusive discounts and offers during this season.",
            };

            _db.SaleEvents.Add(seasonalEvent);
            await _db.SaveChangesAsync();

            return seasonalEvent;
        }
        private string GetSeasonBanner(SeasonType season)
        {
            var banner = $"https://res.cloudinary.com/dje3seaqj/image/upload/v1744799276/{season.ToString().ToLower()}.jpg";
            return banner;
        }
        public void Run(SaleEvent upcomingEvent, TimeSpan timeUntilEvent)
        {
            _backgroundJob.Schedule<ISaleEventService>(
                x => x.ActivateEvent(upcomingEvent.Id),
                timeUntilEvent);
        }
    }
}
