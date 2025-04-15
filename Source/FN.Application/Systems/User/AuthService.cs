using AutoMapper;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.Application.Systems.Token;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Token;
using FN.ViewModel.Systems.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using RestSharp;
using System.Net;

namespace FN.Application.Systems.User
{
    public class AuthService : IAuthService
    {
        private readonly IRedisService _redisService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        public AuthService(IRedisService redisService,
                        IMongoDatabase database,
                        ITokenService tokenService,
                        IMapper mapper,
                        IImageService imageService,
                        UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
            _redisService = redisService;
            _userManager = userManager;
            _tokenService = tokenService;
        }
        public async Task<ApiResult<TokenResponse>> Authenticate(LoginDTO request, HttpContext context)
        {
            try
            {
                var ipAddress = GetPublicIPAddress(context);
                if (string.IsNullOrEmpty(ipAddress)) return new ApiErrorResult<TokenResponse>("Đăng nhập bất thường");
                var user = await _userManager.FindByNameAsync(request.UserName).ConfigureAwait(false);
                if (user == null) return new ApiErrorResult<TokenResponse>("Tài khoản không chính xác");

                var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true).ConfigureAwait(false);
                if (!result.Succeeded) return new ApiErrorResult<TokenResponse>("Tài khoản mật khẩu không chính xác");

                // Tạo token và refresh token
                var clientId = string.IsNullOrEmpty(request.ClientId) ? Guid.NewGuid().ToString() : request.ClientId;

                var expires = request.RememberMe ? DateTime.Now.AddDays(14) : DateTime.Now.AddDays(3);
                var (accessToken, refreshToken) = await GenerateTokensAsync(user, clientId, expires).ConfigureAwait(false);

                // Publish message async (fire-and-forget)
                _ = PublishLoginEventAsync(user, ipAddress, request.UserAgent, clientId);

                var response = new TokenResponse
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken,
                    RefreshTokenExpiry = expires,
                    ClientId = clientId
                };

                return new ApiSuccessResult<TokenResponse>(response);
            }
            catch (Exception e)
            {
                return new ApiErrorResult<TokenResponse>(e.Message);
            }
        }
        private async Task<(string AccessToken, string RefreshToken)> GenerateTokensAsync(AppUser user, string clientId, DateTime expires)
        {
            var accessTokenTask = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();
            var tokenReq = new TokenRequest { UserId = user.Id, ClientId = clientId };
            var expiryDuration = expires - DateTime.Now;

            await _tokenService.SaveRefreshToken(refreshToken, tokenReq, expiryDuration).ConfigureAwait(false);
            return (await accessTokenTask.ConfigureAwait(false), refreshToken);
        }
        private async Task PublishLoginEventAsync(AppUser user, string ipAddress, string userAgent, string clientId)
        {
            await _redisService.Publish(SystemConstant.MESSAGE_LOGIN_EVENT, new LoginResponse
            {
                Email = user.Email!,
                Username = user.UserName!,
                Token = new TokenRequest { UserId = user.Id, ClientId = clientId },
                IpAddress = ipAddress,
                UserAgent = userAgent,
            }).ConfigureAwait(false);
        }
        public async Task<ApiResult<bool>> Register(RegisterDTO request)
        {
            try
            {
                var cacheKey = $"user:{request.UserName}";
                var existed = await _redisService.KeyExist(cacheKey);
                if (existed) return new ApiErrorResult<bool>("Tài khoản đã tồn tại");
                if (await _userManager.FindByNameAsync(request.UserName) != null)
                    return new ApiErrorResult<bool>("Tài khoản đã tồn tại");

                var user = new AppUser
                {
                    UserName = request.UserName,
                    FullName = request.FullName,
                    Email = request.Email,
                    Avatar = SystemConstant.AVATAR_DEFAULT,
                    TimeCreated = DateTime.Now
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                var response = new RegisterResponse
                {
                    FullName = user.FullName,
                    Email = user.Email,
                    Status = false
                };
                if (result.Succeeded)
                {
                    await _redisService.SetValue(cacheKey, user.UserName);
                    response.Status = true;
                    await _redisService.Publish(SystemConstant.MESSAGE_REGISTER_EVENT, response);
                    return new ApiSuccessResult<bool>();
                }
                await _redisService.Publish(SystemConstant.MESSAGE_REGISTER_EVENT, response);
                return new ApiErrorResult<bool>("Tạo mới tài khoản không thành công");
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<bool>(ex.Message);
            }
        }
        public async Task<ApiResult<TokenResponse>> RefreshToken(RefreshTokenRequest request)
        {
            try
            {
                // Kiểm tra refresh token hiện tại
                var currentToken = await _tokenService.GetRefreshToken(request);
                if (currentToken == null || currentToken != request.RefreshToken)
                    return new ApiErrorResult<TokenResponse>("Refresh token không hợp lệ");

                // Lấy thông tin user
                var user = await _userManager.FindByIdAsync(request.UserId.ToString());
                if (user == null)
                    return new ApiErrorResult<TokenResponse>("Tài khoản không tồn tại");

                // Tạo token mới và refresh token mới
                var tokenTask = _tokenService.GenerateAccessToken(user);
                var newRefreshToken = _tokenService.GenerateRefreshToken();
                await tokenTask; // Chờ task hoàn thành

                // Xóa refresh token cũ trước khi lưu mới
                await _tokenService.RemoveRefreshToken(request);

                var expiryDate = DateTime.Now.AddDays(3);
                var response = new TokenResponse
                {
                    AccessToken = await tokenTask,
                    RefreshToken = newRefreshToken,
                    RefreshTokenExpiry = expiryDate,
                    ClientId = request.ClientId
                };

                // Lưu refresh token mới
                await _tokenService.SaveRefreshToken(
                    newRefreshToken,
                    request,
                    expiryDate - DateTime.Now);

                return new ApiSuccessResult<TokenResponse>(response);
            }
            catch (Exception ex)
            {
                return new ApiErrorResult<TokenResponse>("Lỗi: " + ex.Message);
            }
        }
        private string GetPublicIPAddress(HttpContext context)
        {
            //try
            //{
            //    // Tạo client và yêu cầu
            //    var client = new RestClient("https://api.ipify.org");
            //    var request = new RestRequest("", Method.Get);

            //    // Thực hiện yêu cầu và lấy kết quả
            //    var response = client.Execute(request);
            //    return response.Content ?? string.Empty;
            //}
            //catch (Exception ex)
            //{
            //    Console.WriteLine("Lỗi khi lấy địa chỉ IP public: " + ex.Message);
            //    return string.Empty;
            //}
            var ipHeaders = new[] { "X-Forwarded-For", "Forwarded", "X-Real-IP" };
            foreach (var header in ipHeaders)
            {
                if (context.Request.Headers.TryGetValue(header, out var headerValue))
                {
                    var ip = headerValue.ToString().Split(',')[0].Trim();
                    if (!string.IsNullOrEmpty(ip))
                        return ip;
                }
            }

            var remoteIp = context.Connection.RemoteIpAddress;
            if (remoteIp != null)
            {
                if (remoteIp.Equals(IPAddress.IPv6Loopback))
                    return "127.0.0.1";

                if (remoteIp.IsIPv4MappedToIPv6)
                    return remoteIp.MapToIPv4().ToString();

                return remoteIp.ToString();
            }

            return string.Empty;
        }
    }
}
