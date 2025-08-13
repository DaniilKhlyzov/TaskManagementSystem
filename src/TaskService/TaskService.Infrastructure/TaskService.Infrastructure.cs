using Microsoft.EntityFrameworkCore;
using TaskService.Domain;

namespace TaskService.Infrastructure;

/// <summary>
/// Контекст БД задач
/// </summary>
public class JobsDbContext : DbContext
{
    public JobsDbContext(DbContextOptions<JobsDbContext> options) : base(options)
    {
    }

    public DbSet<Job> Jobs => Set<Job>();
    public DbSet<JobHistory> Histories => Set<JobHistory>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Job>(b =>
        {
            b.HasKey(x => x.Id);
            b.Property(x => x.Title).IsRequired().HasMaxLength(200);
            b.HasIndex(x => new { x.IsDeleted, x.Status });
            b.HasIndex(x => x.AssigneeId);
        });

        modelBuilder.Entity<JobHistory>(b =>
        {
            b.HasKey(x => x.Id);
            b.HasIndex(x => x.JobId);
        });
    }
}
