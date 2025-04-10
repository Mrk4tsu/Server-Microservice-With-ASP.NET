using AutoMapper;
using FN.Application.Helper.Devices;
using FN.Application.Helper.Images;
using FN.Application.Systems.Redis;
using FN.DataAccess.Entities;
using FN.Utilities;
using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using System.Net;

namespace FN.Application.Systems.User
{
    public class UserService : IUserService
    {
        private const string ROOT = "user";
        private readonly IRedisService _redisService;
        private readonly UserManager<AppUser> _userManager;
        private readonly IMapper _mapper;
        private readonly IDeviceService _deviceService;
        private readonly IImageService _imageService;
        private readonly ILogger<UserService> _logger;
        public UserService(
         IRedisService redisService,
         UserManager<AppUser> userManager,
         IMapper mapper,
         IImageService imageService,
         IDeviceService deviceService,
         ILogger<UserService> logger)
        {
            _redisService = redisService;
            _userManager = userManager;
            _mapper = mapper;
            _imageService = imageService;
            _deviceService = deviceService;
            _logger = logger;
        }
        #region Helper Methods
        private async Task<AppUser?> GetUserByIdAsync(int id)
        {
            try
            {
                return await _userManager.FindByIdAsync(id.ToString());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by ID {UserId}", id);
                return null;
            }
        }
        private async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            try
            {
                return await _userManager.FindByNameAsync(username);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting user by username {Username}", username);
                return null;
            }
        }
        private async Task InvalidateUserCache(string username)
        {
            try
            {
                var keyCache = SystemConstant.CACHE_USER_BY_USERNAME + username;
                await _redisService.RemoveValue(keyCache);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error invalidating cache for user {Username}", username);
            }
        }
        #endregion
        public async Task<ApiResult<UserViewModel>> GetById(int id)
        {
            var user = await GetUserByIdAsync(id);
            if (user == null)
                return new ApiErrorResult<UserViewModel>("Tài khoản không tồn tại");

            return new ApiSuccessResult<UserViewModel>(_mapper.Map<UserViewModel>(user));
        }
        public async Task<ApiResult<UserViewModel>> GetByUsername(string username)
        {
            try
            {
                var keyCache = SystemConstant.CACHE_USER_BY_USERNAME + username;
                if (await _redisService.KeyExist(keyCache))
                {
                    var cachedUser = await _redisService.GetValue<UserViewModel>(keyCache);
                    if (cachedUser != null)
                    {
                        return new ApiSuccessResult<UserViewModel>(cachedUser);
                    }
                }
                var user = await GetUserByUsernameAsync(username);
                if (user == null)
                    return new ApiErrorResult<UserViewModel>("Tài khoản không tồn tại");
                var userVm = _mapper.Map<UserViewModel>(user);
                await _redisService.SetValue(keyCache, userVm, TimeSpan.FromMinutes(5));
                return new ApiSuccessResult<UserViewModel>(userVm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetByUsername for {Username}", username);
                return new ApiErrorResult<UserViewModel>("Lỗi hệ thống");
            }
        }
        public async Task<ApiResult<string>> RequestUpdateMail(int userId, string newEmail)
        {
            try
            {
                var user = await GetUserByIdAsync(userId);
                if (user == null)
                    return new ApiErrorResult<string>("Tài khoản không tồn tại");

                if (string.Equals(newEmail, user.Email, StringComparison.OrdinalIgnoreCase))
                    return new ApiErrorResult<string>("Email mới không thể trùng với email hiện tại");

                var token = await _userManager.GenerateChangeEmailTokenAsync(user, newEmail);

                await _redisService.Publish(SystemConstant.MESSAGE_UPDATE_EMAIL_EVENT, new UpdateEmailResponse
                {
                    UserId = userId,
                    NewEmail = newEmail,
                    Token = token
                });

                return new ApiSuccessResult<string>(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RequestUpdateMail for user {UserId}", userId);
                return new ApiErrorResult<string>("Lỗi hệ thống");
            }
        }
        public async Task<ApiResult<bool>> ConfirmEmailChange(UpdateEmailResponse response)
        {
            var user = await _userManager.FindByIdAsync(response.UserId.ToString());
            if (user == null) return new ApiErrorResult<bool>("Tài khoản không tồn tại");
            var decodedToken = WebUtility.UrlDecode(response.Token);
            var result = await _userManager.ChangeEmailAsync(user, response.NewEmail, decodedToken);
            if (result.Succeeded)
                return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Xác nhận thay đổi email không thành công");
        }
        public async Task<ApiResult<bool>> UpdateAvatar(int userId, IFormFile file)
        {
            try
            {
                var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                    return new ApiErrorResult<bool>("Tài khoản không tồn tại");

                var newAvatar = await _imageService.UploadImage(file, user.UserName!, user.UserName!, ROOT);
                if (string.IsNullOrEmpty(newAvatar))
                    return new ApiErrorResult<bool>("Không thể lấy dữ liệu ảnh tải lên");

                user.Avatar = newAvatar;
                var result = await _userManager.UpdateAsync(user);

                if (result.Succeeded)
                {
                    await InvalidateUserCache(user.UserName!);
                    return new ApiSuccessResult<bool>();
                }

                return new ApiErrorResult<bool>("Cập nhật ảnh đại diện không thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateAvatar for user {UserId}", userId);
                return new ApiErrorResult<bool>("Lỗi hệ thống");
            }
        }

        #region[Quên mật khẩu]
        public async Task<ApiResult<string>> RequestForgotPassword(RequestForgot request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null) return new ApiErrorResult<string>("User không tồn tại");
            if (request.Email != user.Email) return new ApiErrorResult<string>("Email yêu cầu khôi phục không chính xác");
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            await _redisService.Publish(SystemConstant.MESSAGE_FORGOT_PASSWORD_EVENT, new ForgotPasswordResponse
            {
                Username = user.UserName!,
                Email = user.Email!,
                Token = token
            });
            return new ApiSuccessResult<string>(token);
        }
        public async Task<ApiResult<bool>> ResetPassword(ForgotPasswordRequest request)
        {
            var user = await _userManager.FindByNameAsync(request.Username);
            if (user == null) return new ApiErrorResult<bool>("User không tồn tại");
            var decodedToken = WebUtility.UrlDecode(request.Token);
            var result = await _userManager.ResetPasswordAsync(user, decodedToken, request.NewPassword);
            if (result.Succeeded)
            {
                await _deviceService.RemoveAllDevice(user.Id);
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>(result.Errors.First().Description);
        }
        #endregion
        public async Task<ApiResult<bool>> ChangePassword(ChangePasswordRequest request, int userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new ApiErrorResult<bool>("User không tồn tại");

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);
            if (result.Succeeded)
            {
                if (request.LogoutEverywhere)
                    await _deviceService.RemoveAllDevice(userId);
                return new ApiSuccessResult<bool>();
            }
            return new ApiErrorResult<bool>(result.Errors.First().Description);
        }
        public async Task<ApiResult<bool>> ChangeName(int userId, string newName)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null) return new ApiErrorResult<bool>("User không tồn tại");
            user.FullName = newName;
            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded) return new ApiSuccessResult<bool>();
            return new ApiErrorResult<bool>("Thay đổi tên không thành công");
        }
        private async Task RemoveCache(string key)
        {
            await _redisService.RemoveValue(key);
        }
    }
}
