using Papyrus.Docs.DocumentApi.Services.Repositories;

namespace Papyrus.Docs.DocumentApi.Extensions
{
    public static class ServiceExtension
    {
        public static void AddDbContextExtension(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });
        }

        public static void AddRepositoriesExtension(this IServiceCollection services)
        {
            services.AddScoped<IMeetingMinutesService, MeetingMinutesService>();
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
