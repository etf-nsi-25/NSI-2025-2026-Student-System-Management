using MediatR;
using Analytics.Core.Entities;
using Analytics.Infrastructure.Db;
using Faculty.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Analytics.Infrastructure
{
    public class GetUniversityMetricsHandler
        : IRequestHandler<GetUniversityMetricsQuery, UniversityMetricsDto>
    {
        private readonly AnalyticsDbContext _analyticsDb;
        private readonly IFacultyMetricsRepository _facultyMetrics;

        public GetUniversityMetricsHandler(
            AnalyticsDbContext analyticsDb,
            IFacultyMetricsRepository facultyMetrics)
        {
            _analyticsDb = analyticsDb;
            _facultyMetrics = facultyMetrics;
        }

        public async Task<UniversityMetricsDto> Handle(
            GetUniversityMetricsQuery request,
            CancellationToken cancellationToken)
        {
            // Provjeri cache
            var today = DateTime.UtcNow.Date;
            var existing = await _analyticsDb.Metrics
                .Where(m => m.Timestamp.Date == today)
                .ToListAsync(cancellationToken);

            if (existing.Any())
            {
                return new UniversityMetricsDto
                {
                    StudentsCount = existing.FirstOrDefault(m => m.Name == "StudentsCount")?.Value ?? 0,
                    EmployeesCount = existing.FirstOrDefault(m => m.Name == "EmployeesCount")?.Value ?? 0,
                    CoursesCount = existing.FirstOrDefault(m => m.Name == "CoursesCount")?.Value ?? 0,
                    ActivityCount = existing.FirstOrDefault(m => m.Name == "ActivityCount")?.Value ?? 0
                };
            }

            // Računaj za sve fakultete
            var students = await _facultyMetrics.GetStudentsCountAsync();
            var employees = await _facultyMetrics.GetEmployeesCountAsync();
            var courses = await _facultyMetrics.GetCoursesCountAsync();
            var activities = await _facultyMetrics.GetActivitiesCountAsync();

            // Spremi u Metrics tabelu
            var metrics = new List<Metric>
            {
                new Metric { Name = "StudentsCount", Value = students, Timestamp = DateTime.UtcNow, FacultyId = 0, UserId = "System" },
                new Metric { Name = "EmployeesCount", Value = employees, Timestamp = DateTime.UtcNow, FacultyId = 0, UserId = "System" },
                new Metric { Name = "CoursesCount", Value = courses, Timestamp = DateTime.UtcNow, FacultyId = 0, UserId = "System" },
                new Metric { Name = "ActivityCount", Value = activities, Timestamp = DateTime.UtcNow, FacultyId = 0, UserId = "System" }
            };

            _analyticsDb.Metrics.AddRange(metrics);
            await _analyticsDb.SaveChangesAsync(cancellationToken);

            return new UniversityMetricsDto
            {
                StudentsCount = students,
                EmployeesCount = employees,
                CoursesCount = courses,
                ActivityCount = activities
            };
        }
    }
}
