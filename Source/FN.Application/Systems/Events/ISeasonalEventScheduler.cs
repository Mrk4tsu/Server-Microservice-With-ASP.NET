using FN.DataAccess.Entities;

namespace FN.Application.Systems.Events
{
    public interface ISeasonalEventScheduler
    {
        Task ScheduleSeasonalEvents();
        Task<SaleEvent> GetCurrentSeasonalEvent();
        Task<SaleEvent> GetUpcomingSeasonalEvent();
        void Run(SaleEvent upcomingEvent, TimeSpan timeUntilEvent);
    }
}
