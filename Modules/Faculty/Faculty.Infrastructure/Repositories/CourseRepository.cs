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

        public async Task<Course> AddAsync(Course course)
        {
            course.Id = Guid.NewGuid();
            course.FacultyId = _tenantService.GetCurrentFacultyId();
            course.CreatedAt = DateTime.UtcNow;

            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public Task<Course?> GetByIdAsync(Guid id)
            => Task.FromResult(_context.Courses.FirstOrDefault(x => x.Id == id));

        public Task<List<Course>> GetAllAsync()
            => Task.FromResult(_context.Courses.ToList());

        public async Task<List<Course>> GetByTeacherUserIdAsync(string userId)
        {
            var teacher = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacher == null)
            {
                return new List<Course>();
            }

            return await _context.CourseAssignments
                .Where(ca => ca.TeacherId == teacher.Id)
                .Select(ca => ca.Course)
                .Distinct()
                .ToListAsync();
        }

        public async Task<Course?> UpdateAsync(Course course)
        {
            var existing = _context.Courses.FirstOrDefault(x => x.Id == course.Id);
            if (existing == null)
                return null;

            existing.Name = course.Name;
            existing.Code = course.Code;
            existing.Type = course.Type;
            existing.ProgramId = course.ProgramId;
            existing.ECTS = course.ECTS;
            existing.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return existing;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existing = _context.Courses.FirstOrDefault(x => x.Id == id);
            if (existing == null)
                return false;

            _context.Courses.Remove(existing);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
