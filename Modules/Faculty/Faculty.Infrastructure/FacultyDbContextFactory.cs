using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Infrastructure
{
	public class FacultyDbContextFactory : IDesignTimeDbContextFactory<FacultyDbContext>
	{
		public FacultyDbContext CreateDbContext(string[] args)
		{
			var optionsBuilder = new DbContextOptionsBuilder<FacultyDbContext>();

			var connectionString =
				Environment.GetEnvironmentVariable("DB_CONNECTION")
				?? throw new InvalidOperationException("Environment variable DB_CONNECTION is not set.");

			optionsBuilder.UseNpgsql(connectionString);

			return new FacultyDbContext(optionsBuilder.Options);
		}
	}
}
