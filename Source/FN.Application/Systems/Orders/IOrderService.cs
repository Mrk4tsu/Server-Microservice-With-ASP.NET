using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Order;
using Microsoft.AspNetCore.Http;

namespace FN.Application.Systems.Orders
{
    public interface IOrderService
    {
        Task<ApiResult<bool>> CreateOrder(int userId, OrderCreateRequest request);
        ApiResult<string> CreatePaymentUrl(PaymentInformationModel model, HttpContext context);

        Task<ApiResult<PaymentResponseModel>> PaymentExecute(IQueryCollection collections, int userId);
    }
}
