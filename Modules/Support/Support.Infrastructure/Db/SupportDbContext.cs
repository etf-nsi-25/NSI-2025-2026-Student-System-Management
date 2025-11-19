using Microsoft.EntityFrameworkCore;
using Support.Core.Entities;

namespace Support.Infrastructure.Db
{
    public class SupportDbContext : DbContext
    {
        public SupportDbContext(DbContextOptions<SupportDbContext> options)
            : base(options)
        {
        }

        public DbSet<Request> Requests { get; set; }
    }
}
