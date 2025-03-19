using AutoMapper;
using FN.Application.Helper.Devices;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.Application.Systems.Token;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.Utilities.Device;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.Token;
using FN.ViewModel.Systems.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using MongoDB.Driver;
using System.Net;

namespace FN.Application.Systems.User
{
    public class AuthService : IAuthService
    {
        private readonly IRedisService _redisService;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IDeviceService _deviceSevice;
        public AuthService(IRedisService redisService,
                        IMongoDatabase database,
                        ITokenService tokenService,
                        IMapper mapper,
                        IImageService imageService,
                        IDeviceService deviceService,
                        UserManager<AppUser> userManager,
            SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
            _redisService = redisService;
            _userManager = userManager;
            _deviceSevice = deviceService;
            _tokenService = tokenService;
        }
        public async Task<ApiResult<TokenResponse>> Authenticate(LoginDTO request)
        {
            var user = await _userManager.FindByNameAsync(request.UserName);
            if (user == null) return new ApiErrorResult<TokenResponse>("Tài khoản không chính xác");

            var result = await _signInManager.PasswordSignInAsync(user, request.Password, request.RememberMe, true);
            if (!result.Succeeded) return new ApiErrorResult<TokenResponse>("Tài khoản mật khẩu không chính xác");

            string clientId = Guid.NewGuid().ToString();

            var tokenReq = new TokenRequest
            {
                UserId = user.Id,
                ClientId = clientId
            };

            var deviceInfo = Commons.ParseUserAgent(request.UserAgent);
            var device = new DeviceInfoDetail
            {
                ClientId = clientId,
                Browser = deviceInfo.Browser,
                DeviceType = deviceInfo.DeviceType,
                IPAddress = request.IPAddress,
                LastLogin = DateTime.Now,
                OS = deviceInfo.OS
            };

            var publish = new LoginResponse
            {
                UserId = user.Id,
                Email = user.Email!,
                Username = user.UserName!,
                Token = tokenReq,
                DeviceInfo = device,
            };
            await _redisService.Publish(SystemConstant.MESSAGE_LOGIN_EVENT, publish);

            var expires = request.RememberMe ? DateTime.Now.AddDays(14) : DateTime.Now.AddDays(3);
            var tokenTask = _tokenService.GenerateAccessToken(user);
            var refreshToken = _tokenService.GenerateRefreshToken();

            await Task.WhenAll(tokenTask);

            var response = new TokenResponse
            {
                AccessToken = tokenTask.Result,
                RefreshToken = refreshToken,
                RefreshTokenExpiry = expires,
                ClientId = clientId
            };

            await _tokenService.SaveRefreshToken(response.RefreshToken, tokenReq, expires - DateTime.Now);

            return new ApiSuccessResult<TokenResponse>(response);
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
            var currentToken = await _tokenService.GetRefreshToken(request);
            if (currentToken == null || currentToken != request.RefreshToken)
                return new ApiErrorResult<TokenResponse>("Refresh token không hợp lệ");
            var user = await _userManager.FindByIdAsync(request.UserId.ToString());
            if (user == null) return new ApiErrorResult<TokenResponse>("Tài khoản không tồn tại");

            var token = await _tokenService.GenerateAccessToken(user);
            var newRefreshToken = _tokenService.GenerateRefreshToken();

            var response = new TokenResponse
            {
                AccessToken = token,
                RefreshToken = newRefreshToken,
                RefreshTokenExpiry = DateTime.Now.AddDays(3),
                ClientId = request.ClientId
            };
            await _tokenService.SaveRefreshToken(newRefreshToken, request, response.RefreshTokenExpiry - DateTime.Now);
            return new ApiSuccessResult<TokenResponse>(response);
        }
    }
}
