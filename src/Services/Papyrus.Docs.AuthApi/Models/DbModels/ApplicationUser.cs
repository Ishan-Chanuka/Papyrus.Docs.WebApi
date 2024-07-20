using Microsoft.AspNetCore.Identity;

namespace Papyrus.Docs.AuthApi.Models.DbModels
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public required string FirstName { get; set; }
        public required string LastName { get; set; }

        public ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
