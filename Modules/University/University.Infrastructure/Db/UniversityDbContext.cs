using Microsoft.EntityFrameworkCore;
using University.Core.Entities;

namespace University.Infrastructure.Db
{
    public class UniversityDbContext : DbContext
    {
		public UniversityDbContext(DbContextOptions<UniversityDbContext> options)
					: base(options)
		{
		}

		public DbSet<Faculty> Faculties { get; set; } = null!;
        public DbSet<Issue> Issues { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(UniversityDbContext).Assembly);

			base.OnModelCreating(modelBuilder);
		}
	}
}
