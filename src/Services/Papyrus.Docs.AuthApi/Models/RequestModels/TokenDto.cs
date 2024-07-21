using System.ComponentModel.DataAnnotations;

namespace Papyrus.Docs.AuthApi.Models.RequestModels
{
    public class TokenDto
    {
        [Required]
        public string Token { get; set; }

        [Required]
        public string RefreshToken { get; set; }
    }
}
