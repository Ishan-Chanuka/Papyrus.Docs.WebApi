using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Papyrus.Docs.AuthApi.Models.DbModels;

namespace Papyrus.Docs.AuthApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
        IdentityDbContext<ApplicationUser, ApplicationRole, Guid, IdentityUserClaim<Guid>, ApplicationUserRole,
                          IdentityUserLogin<Guid>, IdentityRoleClaim<Guid>, IdentityUserToken<Guid>>(options)
    {
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>(entity =>
            {
                entity.ToTable("AspNetUsers");
            });

            builder.Entity<ApplicationRole>(entity =>
            {
                entity.ToTable("AspNetRoles");
            });

            builder.Entity<ApplicationUserRole>(entity =>
            {
                entity.ToTable("AspNetUserRoles");

                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasOne(e => e.User)
                      .WithMany(e => e.UserRoles)
                      .HasForeignKey(e => e.UserId);

                entity.HasOne(e => e.Role)
                      .WithMany(e => e.UserRoles)
                      .HasForeignKey(e => e.RoleId);
            });
        }
    }
}
