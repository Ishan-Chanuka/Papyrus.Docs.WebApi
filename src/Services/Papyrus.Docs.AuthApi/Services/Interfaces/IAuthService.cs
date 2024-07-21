namespace Papyrus.Docs.AuthApi.Services.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<bool>> RegisterAsync(RegisterDto request);
        Task<ApiResponse<AuthResponseModel>> LoginAsync(LoginDto request);
        Task<ApiResponse<AuthResponseModel>> RefreshTokenAsync(TokenDto request);
    }
}
