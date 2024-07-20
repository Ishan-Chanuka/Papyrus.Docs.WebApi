namespace Papyrus.Docs.AuthApi.Models.ResponseModels
{
    public class AuthResponseModel
    {
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public string RefreshToken { get; set; }
    }
}
