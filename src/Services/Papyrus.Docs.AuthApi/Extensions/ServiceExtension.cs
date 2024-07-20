using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Papyrus.Docs.AuthApi.Data;
using Papyrus.Docs.AuthApi.Models.DbModels;
using Papyrus.Docs.AuthApi.Seeds;
using System.Text;

namespace Papyrus.Docs.AuthApi.Extensions
{
    public static class ServiceExtension
    {
        public static void AddDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));

                services.AddIdentity<ApplicationUser, ApplicationRole>().AddEntityFrameworkStores<ApplicationDbContext>();
            });
        }

        public static void AddAuthenticationExtension(this IServiceCollection services, IConfiguration configuration)
        {
            string? issure = configuration["ApiSettings:Issuer"];
            string? audience = configuration["ApiSettings:Audience"];
            string? key = configuration.GetSection("ApiSettings:SecretKey").Value;

            try
            {
                services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(x =>
                {
                    x.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidIssuer = issure,
                        ValidAudience = audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true
                    };
                });
            }
            catch (Exception)
            {
                throw;
            }
        }

        public static void AddRepositoriesExtension(this IServiceCollection services)
        {

        }

        public static void SeedDataExtension(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var _userManager = serviceScope.ServiceProvider.GetService<UserManager<ApplicationUser>>()!;
            var _roleManager = serviceScope.ServiceProvider.GetService<RoleManager<ApplicationRole>>()!;

            SeedDefaultRole.SeedAsync(_roleManager).Wait();
            SeedDefaultUser.SeedAsync(_userManager).Wait();
        }

        public static void ApplyMigrationsExtension(this IApplicationBuilder app)
        {
            using var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var _context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>()!;
            if (_context.Database.GetPendingMigrations().Any())
            {
                _context.Database.Migrate();
            }
        }
    }
}
