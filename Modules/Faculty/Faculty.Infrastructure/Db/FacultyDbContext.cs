using Faculty.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Db
{
    public class FacultyDbContext : DbContext
    {
		public FacultyDbContext(DbContextOptions<FacultyDbContext> options)
					: base(options)
		{
		}

		public DbSet<Student> Students { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(FacultyDbContext).Assembly);

			base.OnModelCreating(modelBuilder);
		}
	}
}
