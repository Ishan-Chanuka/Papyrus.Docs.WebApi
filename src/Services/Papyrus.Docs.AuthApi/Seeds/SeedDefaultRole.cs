using Microsoft.AspNetCore.Identity;
using Papyrus.Docs.AuthApi.Models.DbModels;
using Papyrus.Docs.AuthApi.Primitives.Enums;

namespace Papyrus.Docs.AuthApi.Seeds
{
    public static class SeedDefaultRole
    {
        /// <summary>
        /// This method is used to seed the default roles.
        /// </summary>
        /// <param name="roleManager"> This is used to manage the roles. </param>
        /// <returns></returns>
        public static async Task SeedAsync(RoleManager<ApplicationRole> roleManager)
        {
            foreach (var role in Enum.GetValues(typeof(UserRoles)))
            {
                var roleName = role.ToString()!;
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new ApplicationRole
                    {
                        Name = roleName,
                        NormalizedName = roleName.ToUpper(),
                        CreatedDate = DateTime.Now,
                    });
                }
            }
        }
    }
}
