using Microsoft.AspNetCore.Mvc;
using Papyrus.Docs.Email.Service.Models;
using Papyrus.Docs.Email.Service.Services.Interfaces;

namespace Papyrus.Docs.AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService, IEmailSenderService emailSender) : ControllerBase
    {
        [HttpPost("register")]
        public async Task<ActionResult<ApiResponse<bool>>> RegisterAsync(RegisterDto request)
        {
            var response = await _authService.RegisterAsync(request);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<AuthResponseModel>>> LoginAsync(LoginDto request)
        {
            var response = await _authService.LoginAsync(request);

            return StatusCode(response.StatusCode, response);
        }

        [HttpPost("refresh-token")]
        public async Task<ActionResult<ApiResponse<AuthResponseModel>>> RefreshTokenAsync(TokenDto request)
        {
            var response = await _authService.RefreshTokenAsync(request);

            return StatusCode(response.StatusCode, response);
        }

        [HttpGet("test-send-email")]
        public async Task<ActionResult<ApiResponse<bool>>> TestEmailService()
        {
            var to = new List<EmailAddress>
            {
                new("Test User", "ishanchanuka3@gmail.com")
            };

            var message = new Message(to, "Test Email", "This is a test email");

            var response = await emailSender.SendEmailAsync(message);

            return Ok(response);
        }
    }
}
