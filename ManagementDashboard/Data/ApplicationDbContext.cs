using Microsoft.EntityFrameworkCore;

namespace ManagementDashboard.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {
        }
        // DbSets for your entities
        public DbSet<Models.User> Users { get; set; } = null!;
        
    }
}
