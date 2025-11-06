using Identity.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Data;

public class IdentityDbContext : DbContext
{
    public IdentityDbContext(DbContextOptions<IdentityDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)

    {

        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseNpgsql(
                "Host=localhost;Port=5432;Database=unsa_sms_db;Username=user;Password=admin");
        }

    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>(entity =>
        {
            entity.ToTable("Users");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(e => e.Email)
                .IsUnique();

            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.TenantId)
                .IsRequired();

            entity.Property(e => e.IsActive)
                .IsRequired()
                .HasDefaultValue(true);

            entity.Property(e => e.CreatedAt)
                .IsRequired();
        });

        // RefreshToken entity configuration
        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.ToTable("RefreshTokens");
            entity.HasKey(e => e.Id);

            entity.Property(e => e.Token)
                .IsRequired()
                .HasMaxLength(256);

            entity.HasIndex(e => e.Token)
                .IsUnique();

            entity.Property(e => e.UserId)
                .IsRequired();

            entity.Property(e => e.ExpiresAt)
                .IsRequired();

            entity.Property(e => e.CreatedAt)
                .IsRequired();

            entity.Property(e => e.IsRevoked)
                .IsRequired()
                .HasDefaultValue(false);

            entity.Property(e => e.IpAddress)
                .HasMaxLength(50);

            entity.Property(e => e.UserAgent)
                .HasMaxLength(500);

            // Relationships
            entity.HasOne(e => e.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}