using Microsoft.EntityFrameworkCore;
using NotificationService.Domain;

namespace NotificationService.Infrastructure;

public class NotificationDbContext : DbContext
{
    public NotificationDbContext(DbContextOptions<NotificationDbContext> options) : base(options) { }

    public DbSet<Notification> Notifications => Set<Notification>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Notification>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => new { x.UserId, x.IsRead });
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.Property(x => x.Message).HasMaxLength(2000);
        });
    }
}
