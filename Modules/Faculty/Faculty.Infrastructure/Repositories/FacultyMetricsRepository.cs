using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Faculty.Infrastructure.Repositories
{
    public class FacultyMetricsRepository : IFacultyMetricsRepository
    {
        private readonly FacultyDbContext _db;

        public FacultyMetricsRepository(FacultyDbContext db)
        {
            _db = db;
        }

        public Task<int> GetStudentsCountAsync()
            => _db.Students.IgnoreQueryFilters().AsNoTracking().CountAsync();

        public Task<int> GetEmployeesCountAsync()
            => _db.Teachers.IgnoreQueryFilters().AsNoTracking().CountAsync();

        public Task<int> GetCoursesCountAsync()
            => _db.Courses.IgnoreQueryFilters().AsNoTracking().CountAsync();

        public Task<int> GetActivitiesCountAsync()
            => _db.Attendances.IgnoreQueryFilters().AsNoTracking().CountAsync();
    }
}
