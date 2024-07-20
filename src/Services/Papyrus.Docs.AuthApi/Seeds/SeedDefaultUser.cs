using Microsoft.AspNetCore.Identity;
using Papyrus.Docs.AuthApi.Models.DbModels;
using Papyrus.Docs.AuthApi.Primitives.Enums;

namespace Papyrus.Docs.AuthApi.Seeds
{
    public static class SeedDefaultUser
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager)
        {
            var defaultUser = new ApplicationUser
            {
                UserName = "Admin",
                Email = "admin@papyrusdocs.com",
                FirstName = "Admin",
                LastName = "Admin",
                PhoneNumber = "",
                NormalizedEmail = "admin@papyrusdocs.com".ToUpper(),
                EmailConfirmed = true,
            };

            if (userManager.Users.All(u => u.Id != defaultUser.Id))
            {
                await userManager.CreateAsync(defaultUser, "Admin@123");
                await userManager.AddToRoleAsync(defaultUser, UserRoles.Admin.ToString());
            }
        }
    }
}
