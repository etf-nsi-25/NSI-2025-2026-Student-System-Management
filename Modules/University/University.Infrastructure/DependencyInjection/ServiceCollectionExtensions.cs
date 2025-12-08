<<<<<<< HEAD
﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using University.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using University.Core.Interfaces;
using University.Infrastructure.Repositories;
=======
﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using University.Infrastructure.Db;
>>>>>>> 28ad086ec194b0f4e021dae55008bf0e637ee75d

namespace University.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
<<<<<<< HEAD
        public static IServiceCollection AddUniversityModule(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Entity Framework
            services.AddDbContext<UniversityDbContext>(options =>
                options.UseNpgsql(
                    configuration.GetConnectionString("Database"),
                    npgsql =>
                    {
                        // Custom migration history table in "university" schema
                        npgsql.MigrationsHistoryTable("__EFMigrationsHistory_University", "university");
                    }
                )
            );

            services.AddScoped<IAcademicYearRepository, AcademicYearRepository>();
            services.AddScoped<IFacultyRepository, FacultyRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IProgramRepository, ProgramRepository>();

            return services;
        }
    }
}
=======
		public static IServiceCollection AddUniversityModule(this IServiceCollection services, IConfiguration configuration)
		{
			services.AddDbContext<UniversityDbContext>(options =>
				options.UseNpgsql(configuration.GetConnectionString("Database")));
			return services;
		}
	}
}
>>>>>>> 28ad086ec194b0f4e021dae55008bf0e637ee75d
