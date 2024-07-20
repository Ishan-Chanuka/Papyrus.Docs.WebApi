using Microsoft.AspNetCore.Identity;

namespace Papyrus.Docs.AuthApi.Models.DbModels
{
    public class ApplicationRole : IdentityRole<Guid>
    {
        public DateTime? CreatedDate { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
