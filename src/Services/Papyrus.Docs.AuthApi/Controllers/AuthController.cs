using Microsoft.AspNetCore.Mvc;

namespace Papyrus.Docs.AuthApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService _authService) : ControllerBase
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
    }
}
