﻿using FN.ViewModel.Systems.Token;

namespace FN.ViewModel.Systems.User
{
    public class RegisterDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string ConfirmPassword { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
    }
    public class RegisterResponse
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public bool Status { get; set; }
    }
    public class LoginDTO
    {
        public string UserName { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public string? ClientId { get; set; } = string.Empty;
        public bool RememberMe { get; set; }
    }
    public class LoginResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public string UserAgent { get; set; } = string.Empty;
        public TokenRequest Token { get; set; } = new();
        //public DeviceInfoDetail DeviceInfo { get; set; } = new DeviceInfoDetail();
    }
    public class UpdateEmailDTO
    {
        public int UserId { get; set; }
        public string NewEmail { get; set; } = string.Empty;
    }
    public class UpdateEmailResponse
    {
        public int UserId { get; set; }
        public string NewEmail { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
    public class MailRequest
    {
        public string Username { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;
    }
    public class ForgotPasswordRequest
    {
        public string Username { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
    }
    public class ChangePasswordRequest
    {
        public string CurrentPassword { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;
        public string ConfirmNewPassword { get; set; } = string.Empty;
        public bool LogoutEverywhere { get; set; } = false;
    }
    public class UserMailTokenResponse
    {
        public string Username { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Token { get; set; } = string.Empty;
    }
    public class VerifyRequest
    {
        public string Token { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
    }
}