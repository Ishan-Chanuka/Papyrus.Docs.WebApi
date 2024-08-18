using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Papyrus.Docs.AuthApi.Data;
using Papyrus.Docs.AuthApi.Middleware;
using Papyrus.Docs.AuthApi.Models.DbModels;
using Papyrus.Docs.AuthApi.Seeds;
using Papyrus.Docs.AuthApi.Services.Repositories;
using Papyrus.Docs.Email.Service.Configurations;
using Papyrus.Docs.Email.Service.Services.Interfaces;
using Papyrus.Docs.Email.Service.Services.Repositories;
using Papyrus.Docs.Email.Service.Templates;

namespace Papyrus.Docs.AuthApi.Extensions
{
    /// <summary>
    /// This class contains the extension methods for the services.
    /// </summary>
    public static class ServiceExtension
    {
        /// <summary>
        /// This method is used to add the database context to the services.
        /// </summary>
        /// <param name="services"> This is used to add the services to the service collection. </param>
        /// <param name="configuration"> This is used to get the connection string from the appsettings.json file. </param>
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

        /// <summary>
        /// This method is used to add the authentication to the services.
        /// </summary>
        /// <param name="services"> This is used to add the services to the service collection. </param>
        /// <param name="configuration"> This is used to get the connection string from the appsettings.json file. </param>
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

        /// <summary>
        /// This method is used to get the email configuration from the appsettings.json file.
        /// </summary>
        /// <param name="services"> This is used to add the services to the service collection. </param>
        /// <param name="configuration"> This is used to get the connection string from the appsettings.json file. </param>
        public static void GetConfigurationExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<EmailConfiguration>(configuration.GetSection("EmailConfiguration"));
            services.AddSingleton(sp => sp.GetRequiredService<IOptions<EmailConfiguration>>().Value);
        }

        /// <summary>
        /// This method is used to add the repositories to the services.
        /// </summary>
        /// <param name="services"> This is used to add the services to the service collection. </param>
        public static void AddRepositoriesExtension(this IServiceCollection services)
        {
            services.AddScoped<IAuthService, AuthService>();
            services.AddTransient<RazorViewToStringRenderer>();
            services.AddTransient<IEmailSenderService, EmailSenderService>();
        }

        /// <summary>
        /// This method is used to add the error handle middleware to the services.
        /// </summary>
        /// <param name="app"> This is used to add the middleware to the application. </param>
        public static void AddErrorHanldeMiddlewareExtension(this IApplicationBuilder app)
        {
            app.UseMiddleware<ErrorHanldeMiddleware>();
        }

        /// <summary>
        /// This method is used to seed the data to the database.
        /// </summary>
        /// <param name="services"> This is used to get the services from the service provider. </param>
        /// <returns></returns>
        public static async Task SeedDataExtension(this IServiceProvider services)
        {
            var _roleManager = services.GetRequiredService<RoleManager<ApplicationRole>>();
            var _userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            await SeedDefaultRole.SeedAsync(_roleManager);
            await SeedDefaultUser.SeedAsync(_userManager);
        }

        /// <summary>
        /// This method is used to apply the migrations to the database.
        /// </summary>
        /// <param name="services"> This is used to get the services from the service provider. </param>
        /// <returns></returns>
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
