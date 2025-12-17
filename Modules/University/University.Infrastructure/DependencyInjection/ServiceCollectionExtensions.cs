using Microsoft.Extensions.DependencyInjection;
using University.Application.Interfaces;
using University.Application.Services;
using University.Core.Interfaces.Repositories;
using University.Infrastructure.Db.Repositories;
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
			services.AddScoped<IIssueRepository, IssueRepository>();
			services.AddScoped<ICategoryRepository, CategoryRepository>();
			return services;
		}
	}
}
