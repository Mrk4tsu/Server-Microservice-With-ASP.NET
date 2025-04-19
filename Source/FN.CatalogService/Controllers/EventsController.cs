using FN.Application.Systems.Events;
using FN.ViewModel.Systems.Events;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace FN.CatalogService.Controllers
{
    [Route("api/event")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly ISaleEventService _eventService;
        private readonly ISeasonalEventScheduler _seasonalEvent;
        public EventsController(ISeasonalEventScheduler seasonalEvent,
            ISaleEventService eventService)
        {
            _eventService = eventService;
            _seasonalEvent = seasonalEvent;
        }
        [HttpGet("current-event"), AllowAnonymous]
        public async Task<IActionResult> GetCurrentEvent()
        {
            var result = await _seasonalEvent.GetCurrentSeasonalEvent();
            return Ok(result);
        }
        [HttpPost("create-event")]
        public async Task<IActionResult> CreateEvent([FromBody] EventCreateOrUpdateRequest request)
        {
            var result = await _eventService.CreateEvent(request);
            return Ok(result);
        }
        [HttpPost("add-product-event")]
        public async Task<IActionResult> AddProductToEvent([FromBody] EventProductRequest request)
        {
            var result = await _eventService.AddProductToEvent(request);
            return Ok(result);
        }
        [HttpPost("add-product-event-2"), AllowAnonymous]
        public async Task<IActionResult> AddProductsToEvent([FromBody] AddProductsToEventRequest request, int eventId)
        {
            var result = await _eventService.AddProductToEvent(request, eventId);
            return Ok(result);
        }
        [HttpGet("list-product-event"), AllowAnonymous]
        public async Task<IActionResult> GetProductsInEvent()
        {
            var result = await _eventService.GetActiveEventProducts();
            return Ok(result);
        }
    }
}
