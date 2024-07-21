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
