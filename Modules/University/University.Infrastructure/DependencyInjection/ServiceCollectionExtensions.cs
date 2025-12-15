using Microsoft.Extensions.DependencyInjection;
using University.Application.Interfaces;
using University.Application.Services;
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using University.Infrastructure.Db;

namespace University.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
		public static IServiceCollection AddUniversityModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<UniversityDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("Database")));
      
			services.AddScoped<IFacultyService, FacultyService>();
			return services;
		}
	}
}
