using FN.Utilities.Device;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Token;
using FN.ViewModel.Systems.User;
using Microsoft.AspNetCore.Http;

namespace FN.Application.Systems.User
{
    public interface IAuthService
    {
        Task<ApiResult<TokenResponse>> RefreshToken(RefreshTokenRequest request);
        Task<ApiResult<bool>> Register(RegisterDTO request);
        Task<ApiResult<TokenResponse>> Authenticate(LoginDTO request, HttpContext context);
        Task<bool> IsJustSendMail(int userId);
    }
}
