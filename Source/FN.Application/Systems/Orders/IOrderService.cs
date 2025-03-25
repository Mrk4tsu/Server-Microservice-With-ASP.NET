using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Order;
using Microsoft.AspNetCore.Http;

namespace FN.Application.Systems.Orders
{
    public interface IOrderService
    {
        Task<ApiResult<int>> CreateOrder(int userId, OrderCreateRequest request);
        Task<ApiResult<string>> CreatePaymentUrl(PaymentInformationModel model, HttpContext context, int orderId);

        Task<ApiResult<PaymentResponseModel>> PaymentExecute(IQueryCollection collections, int userId);
    }
}
