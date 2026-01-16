using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Http;
using Faculty.Infrastructure.Mappers;
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

            var courseSchema = CourseMapper.ToPersistence(course);
            _context.Courses.Add(courseSchema);
            await _context.SaveChangesAsync();
            return CourseMapper.ToDomain(courseSchema, includeRelationships: false);
        }

        public async Task<Course?> GetByIdAsync(Guid id)
        {
            var courseSchema = await _context.Courses
                .FirstOrDefaultAsync(x => x.Id == id);

            return courseSchema != null
                ? CourseMapper.ToDomain(courseSchema, includeRelationships: false)
                : null;
        }

        public async Task<List<Course>> GetAllAsync()
        {
            var courseSchemas = await _context.Courses.ToListAsync();
            return CourseMapper.ToDomainCollection(courseSchemas, includeRelationships: false).ToList();
        }

        public async Task<List<(Course Course, int StudentCount)>> GetProfessorCoursesWithStudentCountAsync(string userId)
        {
            var teacherSchema = await _context.Teachers.FirstOrDefaultAsync(t => t.UserId == userId);
            if (teacherSchema == null)
            {
                return new List<(Course, int)>();
            }

            var courseIds = await _context.CourseAssignments
                .Where(ca => ca.TeacherId == teacherSchema.Id)
                .Select(ca => ca.CourseId)
                .Distinct()
                .ToListAsync();

            var coursesWithCount = await _context.Courses
                .Where(c => courseIds.Contains(c.Id))
                .Select(c => new
                {
                    Course = c,
                    StudentCount = c.Enrollments.Count
                })
                .ToListAsync();

            return coursesWithCount
                .Select(x => (CourseMapper.ToDomain(x.Course, includeRelationships: false), x.StudentCount))
                .ToList();
        }

        public async Task<Course?> UpdateAsync(Course course)
        {
            var existingSchema = await _context.Courses.FirstOrDefaultAsync(x => x.Id == course.Id);
            if (existingSchema == null)
                return null;

            CourseMapper.UpdatePersistence(existingSchema, course);
            existingSchema.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return CourseMapper.ToDomain(existingSchema, includeRelationships: false);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingSchema = await _context.Courses.FirstOrDefaultAsync(x => x.Id == id);
            if (existingSchema == null)
                return false;

            _context.Courses.Remove(existingSchema);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
