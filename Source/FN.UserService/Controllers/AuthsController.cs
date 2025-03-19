using FN.Application.Helper.Devices;
using FN.Application.Systems.User;
using FN.ViewModel.Systems.Token;
using FN.ViewModel.Systems.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FN.UserService.Controllers
{
    [Route("api/auth")]
    [ApiController]
    public class AuthsController : BasesController
    {
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IDeviceService _deviceService;
        public AuthsController(IUserService userService, IAuthService authService, IDeviceService deviceService)
        {
            _authService = authService;
            _userService = userService;
            _deviceService = deviceService;
        }
        [HttpPost("register"), AllowAnonymous]
        public async Task<IActionResult> Register(RegisterDTO register)
        {
            var result = await _authService.Register(register);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("login"), AllowAnonymous]
        public async Task<IActionResult> Login(LoginDTO login)
        {
            var result = await _authService.Authenticate(login);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("logout"), AllowAnonymous]
        public async Task<IActionResult> Logout(TokenRequest request)
        {
            var result = await _deviceService.RevokeDevice(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpGet("devices")]
        public async Task<IActionResult> ListDevice()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var result = await _deviceService.GetRegisteredDevices(userId.Value);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("revoke-device")]
        public async Task<IActionResult> RevokeDevice(string clientId)
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();

            var request = new TokenRequest
            {
                UserId = userId.Value,
                ClientId = clientId
            };
            var result = await _deviceService.RevokeDevice(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("revoke-all-devices")]
        public async Task<IActionResult> RevokeAllDevice()
        {
            var userId = GetUserIdFromClaims();
            if (userId == null) return Unauthorized();
            var request = new TokenRequest
            {
                UserId = userId.Value
            };
            var result  = await _deviceService.RemoveAllDevice(userId.Value);

            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
        [HttpPost("refresh-token"), AllowAnonymous]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var result = await _authService.RefreshToken(request);
            if (result.Success)
                return Ok(result);
            return BadRequest(result);
        }
    }
}
