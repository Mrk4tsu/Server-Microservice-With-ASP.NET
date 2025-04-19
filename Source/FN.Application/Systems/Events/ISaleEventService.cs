using FN.DataAccess.Entities;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Events;

namespace FN.Application.Systems.Events
{
    public interface ISaleEventService
    {
        Task<ApiResult<bool>> AddProductToEvent(EventProductRequest request);
        Task<ApiResult<bool>> AddProductToEvent(AddProductsToEventRequest request, int eventId);
        Task<ApiResult<int>> CreateEvent(EventCreateOrUpdateRequest request);
        Task ActivateEvent(int eventId);
        Task DeactivateEvent(int eventId);
        Task<ApiResult<List<EventProductResponse>>> GetActiveEventProducts();
        Task<ProductPrice?> GetCurrentEventPrice(int productId);
        Task<ApiResult<int>> ProcessEventPurchase(int eventProductId);
    }
}
