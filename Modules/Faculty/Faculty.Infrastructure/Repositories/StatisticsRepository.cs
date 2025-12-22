// StatisticsRepository.cs
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Faculty.Infrastructure.Repositories
{
    public class StatisticsRepository : IStatisticsRepository
    {
        private readonly FacultyDbContext _context;

        public StatisticsRepository(FacultyDbContext context)
        {
            _context = context; // koristi postojeći DbContext
        }

        public async Task<int> GetStudentsCountAsync()
        {
            return await _context.Students.IgnoreQueryFilters().CountAsync();
        }

        public async Task<int> GetEmployeesCountAsync()
        {
            return await _context.Teachers.IgnoreQueryFilters().CountAsync();
        }

        public async Task<int> GetCoursesCountAsync()
        {
            return await _context.Courses.IgnoreQueryFilters().CountAsync();
        }

        public async Task<int> GetActivitiesCountAsync()
        {
            return await _context.CourseAssignments.IgnoreQueryFilters().CountAsync();
        }

    }
}
