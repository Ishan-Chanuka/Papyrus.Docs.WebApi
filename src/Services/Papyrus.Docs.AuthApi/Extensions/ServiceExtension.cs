using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Papyrus.Docs.AuthApi.Data;
using Papyrus.Docs.AuthApi.Middleware;
using Papyrus.Docs.AuthApi.Models.DbModels;
using Papyrus.Docs.AuthApi.Seeds;
using Papyrus.Docs.AuthApi.Services.Repositories;

namespace Papyrus.Docs.AuthApi.Extensions
{
    public static class ServiceExtension
    {
        public static void AddDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddIdentity<ApplicationUser, ApplicationRole>()
                    .AddEntityFrameworkStores<ApplicationDbContext>()
                    .AddDefaultTokenProviders();
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
            services.AddScoped<IAuthService, AuthService>();
        }

        public static void AddErrorHanldeMiddlewareExtension(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHanldeMiddleware>();
        }

        public static async Task SeedDataExtension(this IServiceProvider services)
        {
            var _roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedDefaultRole.SeedAsync(_roleManager);
            await SeedDefaultUser.SeedAsync(_userManager);
        }

        public static async Task ApplyMigrationsExtension(this IServiceProvider services)
        {
            using var serviceScope = services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            var _context = serviceScope.ServiceProvider.GetService<ApplicationDbContext>()!;
            if (_context.Database.GetPendingMigrations().Any())
            {
                await _context.Database.MigrateAsync();
            }
        }
    }
}
