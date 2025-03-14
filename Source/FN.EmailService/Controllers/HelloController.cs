using FN.Application.Systems.Redis;
using FN.Utilities;
using FN.ViewModel.Systems.User;
using Microsoft.AspNetCore.Mvc;

namespace FN.EmailService.Controllers
{
    [Route("api/email")]
    [ApiController]
    public class HelloController : ControllerBase
    {
        private readonly IRedisService _service;
        public HelloController(IRedisService service)
        {
            _service = service;
        }
        [HttpGet("hello")]
        public IActionResult Hello()
        {
            return Ok("Hello from Email Service");
        }
        [HttpPost("send")]
        public async Task<IActionResult> SendEmail()
        {
            var response = new RegisterResponse
            {
                FullName = "Test Deploy",
                Email = "mrk4tsu@gmail.com",
                Status = false
            };
            await _service.Publish(SystemConstant.MESSAGE_REGISTER_EVENT, response);
            return Ok("Email sent");
        }
    }
}
