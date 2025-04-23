using FN.Application.Catalog.Product.Notifications;
using FN.Application.Systems.User;
using FN.ViewModel.Systems.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace FN.UserService.Controllers
{
    [Route("api/user")]
    [ApiController, Authorize]
    public class UsersController : BasesController
    {
        private readonly IUserService _userService;
        private readonly INotifyService _notifyService;
        public UsersController(IUserService userService, INotifyService notifyService)
        {
            _userService = userService;
            _notifyService = notifyService;
        }
        [HttpGet("{userId}"), Authorize(Roles = "Admin")]
        public async Task<IActionResult> Get(int userId)
        {
            var result = await _userService.GetById(userId);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("profile"), AllowAnonymous]
        public async Task<IActionResult> GetByUsername(string username)
        {
            var result = await _userService.GetByUsername(username);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword(ChangePasswordRequest request)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _userService.ChangePassword(request, userId.Value);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("request-forgot"), AllowAnonymous]
        public async Task<IActionResult> ForgotPassword(MailRequest request)
        {
            var result = await _userService.RequestForgotPassword(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("reset-password"), AllowAnonymous]
        public async Task<IActionResult> ResetPassword(ForgotPasswordRequest request)
        {
            var result = await _userService.ResetPassword(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("confirm-email"), AllowAnonymous]
        public async Task<IActionResult> ConfirmEmailChange(UpdateEmailResponse response)
        {
            var result = await _userService.ConfirmEmailChange(response);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("request-email")]
        public async Task<IActionResult> RequestUpdateMail(string newEmail)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var requestResult = await _userService.RequestUpdateMail(userId.Value, newEmail);
            //await _redisService.Publish(SystemConstant.MESSAGE_UPDATE_EMAIL_EVENT, new UpdateEmailResponse
            //{
            //    UserId = userId.Value,
            //    NewEmail = newEmail,
            //    Token = requestResult.Data!
            //});
            if (!requestResult.Success)
                return BadRequest(requestResult);
            return Ok(requestResult);
        }
        [HttpPut("avatar")]
        public async Task<IActionResult> UpdateAvatar(IFormFile file)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _userService.UpdateAvatar(userId.Value, file);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPut("change-name")]
        public async Task<IActionResult> ChangeName(string newName)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _userService.ChangeName(userId.Value, newName);
            return Ok(result);

        }
        [HttpGet("notify")]
        public async Task<IActionResult> GetNotify()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _notifyService.ListNotify(userId.Value);
            return Ok(result);
        }
        [HttpPost("request-verify")]
        public async Task<IActionResult> RequestVerifyEmail()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _userService.RequestVerifyEmail(userId.Value);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("confirm-verify"), AllowAnonymous]
        public async Task<IActionResult> ConfirmVerifyEmail(VerifyRequest response)
        {
            var result = await _userService.ConfirmVerifyEmail(response);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }


    }
}
