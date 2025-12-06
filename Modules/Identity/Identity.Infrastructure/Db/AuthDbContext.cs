using Identity.Core.Entities;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Identity.Infrastructure.Db
{
    public class AuthDbContext : IdentityDbContext<ApplicationUser>
    {
        public DbSet<User> DomainUsers { get; set; }

        public AuthDbContext(DbContextOptions<AuthDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().ToTable("DomainUsers");
        }
    }
}
