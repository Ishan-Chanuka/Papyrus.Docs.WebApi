namespace Papyrus.Docs.DocumentApi.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
    {
        public DbSet<MeetingMinutes> MeetingMinutes { get; set; }
    }
}
