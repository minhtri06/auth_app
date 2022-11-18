using Microsoft.EntityFrameworkCore;

namespace JwtApplication.Entities
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options) { }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(e =>
            {
                e.ToTable("User");
                e.HasKey("Id");
                e.Property(u => u.UserName).IsRequired().HasMaxLength(50);
                e.Property(u => u.PasswordHash).IsRequired();
                e.Property(u => u.PasswordSalt).IsRequired();
            });

            modelBuilder.Entity<RefreshToken>(e =>
            {
                e.ToTable("RefreshToken");
                e.HasKey("Id");
                e.Property(u => u.Token).IsRequired();
                e.Property(u => u.ExpiredAt).IsRequired();
                e.Property(u => u.CreatedAt).IsRequired();
            });
        }
    }
}
