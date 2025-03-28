using FN.ViewModel.Helper.API;
using FN.ViewModel.Helper.Paging;
using FN.ViewModel.Systems.Order;
using Microsoft.AspNetCore.Http;

namespace FN.Application.Systems.Orders
{
    public interface IOrderService
    {
        Task<ApiResult<int>> CreateOrder(int userId, OrderCreateRequest request);
        Task<ApiResult<string>> CreatePaymentUrl(PaymentInformationModel model, HttpContext context, int orderId);
        Task<ApiResult<PaymentResponseModel>> PaymentExecute(IQueryCollection collections, int userId);
        Task<ApiResult<PagedResult<OrderViewModel>>> GetOrders(int userId, OrderPagingRequest request);
        Task<ApiResult<PagedResult<PaymentViewModel>>> GetPayments(int userId, PaymentPagingRequest reuqest);
    }
}
