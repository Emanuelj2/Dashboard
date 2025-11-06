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
        public DbSet<Models.Message> Messages { get; set; } = null!;

       protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure relationships and constraints if needed
            modelBuilder.Entity<Models.Message>()
                .HasOne(m => m.Sender)
                .WithMany(u => u.SentMessages)
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Models.Message>()
                .HasOne(m => m.Recipient)
                .WithMany(u => u.ReceivedMessages)
                .HasForeignKey(m => m.RecipientId)
                .OnDelete(DeleteBehavior.Restrict);

            //indexes
            modelBuilder.Entity<Models.User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
