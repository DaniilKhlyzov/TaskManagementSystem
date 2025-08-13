using Microsoft.EntityFrameworkCore;
using AuthService.Domain.Entities;

namespace AuthService.Infrastructure.Data
{
    public class AuthDbContext : DbContext
    {
        public AuthDbContext(DbContextOptions<AuthDbContext> options) : base(options) { }

        public DbSet<User> Users => Set<User>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(x => x.Id);
                b.HasIndex(x => x.Email).IsUnique();
                b.Property(x => x.Email).IsRequired().HasMaxLength(100);
                b.Property(x => x.FirstName).IsRequired().HasMaxLength(100);
                b.Property(x => x.LastName).IsRequired().HasMaxLength(100);
                b.Property(x => x.PasswordHash).IsRequired();
            });
        }
    }
}


