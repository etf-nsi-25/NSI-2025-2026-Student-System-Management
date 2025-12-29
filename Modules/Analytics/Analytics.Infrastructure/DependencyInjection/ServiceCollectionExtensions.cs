using Analytics.Application.Interfaces;
using Analytics.Application.Services;
using Analytics.Infrastructure.Db;
using Analytics.Infrastructure.Readers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Analytics.Infrastructure
{
	public static class DependencyInjection
	{
		public static IServiceCollection AddAnalyticsModule(this IServiceCollection services, IConfiguration config)
		{
			services.AddDbContext<AnalyticsDbContext>(opt =>
				opt.UseNpgsql(config.GetConnectionString("Database")));

			services.AddScoped<IFacultyAnalyticsReader, FacultyAnalyticsReader>();

			services.AddScoped<IAcademicPerformanceService, AcademicPerformanceService>();

			return services;
		}
	}
}
