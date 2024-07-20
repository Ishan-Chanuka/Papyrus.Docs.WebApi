using Microsoft.AspNetCore.Identity;

namespace Papyrus.Docs.AuthApi.Models.DbModels
{
    public class ApplicationUserRole : IdentityUserRole<Guid>
    {
        public virtual ApplicationUser User { get; set; }
        public virtual ApplicationRole Role { get; set; }
    }
}
