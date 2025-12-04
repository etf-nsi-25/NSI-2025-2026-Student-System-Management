using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Identity.Core.Entities;


namespace Identity.Infrastructure.Db
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<User> DomainUsers { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("DomainUsers");
        }
    }
}