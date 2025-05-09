﻿using FN.ViewModel.Helper.API;
using FN.ViewModel.Systems.User;
using Microsoft.AspNetCore.Http;

namespace FN.Application.Systems.User
{
    public interface IUserService
    {
        Task<ApiResult<UserViewModel>> GetById(int id);
        Task<ApiResult<UserViewModel>> GetByUsername(string username);
        Task<ApiResult<bool>> UpdateAvatar(int userId, IFormFile file);
        Task<ApiResult<bool>> ConfirmEmailChange(UpdateEmailResponse response);
        Task<ApiResult<string>> RequestUpdateMail(int userId, string newEmail);
        Task<ApiResult<string>> RequestForgotPassword(MailRequest request);
        Task<ApiResult<bool>> ResetPassword(ForgotPasswordRequest request);
        Task<ApiResult<bool>> ChangePassword(ChangePasswordRequest request, int userId);
        Task<ApiResult<bool>> ChangeName(int userId, string newName);
        Task<ApiResult<string>> RequestVerifyEmail(int userId); 
        Task<ApiResult<bool>> ConfirmVerifyEmail(VerifyRequest response);
    }
}
