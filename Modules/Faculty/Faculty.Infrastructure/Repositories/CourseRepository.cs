using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Http;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly FacultyDbContext _context;
        private readonly ITenantService _tenantService;

        public CourseRepository(FacultyDbContext context, ITenantService tenantService)
        {
            _context = context;
            _tenantService = tenantService;
        }

        public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default)
        {
            course.Id = Guid.NewGuid();
            course.FacultyId = _tenantService.GetCurrentFacultyId();
            course.CreatedAt = DateTime.UtcNow;

            await _context.Courses.AddAsync(course, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return course;
        }

        public Task<Course?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
            => _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == id, cancellationToken);

        public Task<List<Course>> GetAllAsync(CancellationToken cancellationToken = default)
            => _context.Courses
                .AsNoTracking()
                .ToListAsync(cancellationToken);

        public async Task<List<Course>> GetByTeacherUserIdAsync(string userId, CancellationToken cancellationToken = default)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId, cancellationToken);
            if (teacher == null)
                return new List<Course>();

            return await _context.CourseAssignments
                .Where(ca => ca.TeacherId == teacher.Id)
                .Select(ca => ca.Course)
                .Distinct()
                .ToListAsync(cancellationToken);
        }

        public async Task<Course?> UpdateAsync(Course course, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Courses.FirstOrDefaultAsync(x => x.Id == course.Id, cancellationToken);
            if (existing == null)
                return null;

            existing.Name = course.Name;
            existing.Code = course.Code;
            existing.Type = course.Type;
            existing.ProgramId = course.ProgramId;
            existing.ECTS = course.ECTS;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync(cancellationToken);
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var existing = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id, cancellationToken);
            if (existing == null)
                return false;

            _context.Courses.Remove(existing);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
