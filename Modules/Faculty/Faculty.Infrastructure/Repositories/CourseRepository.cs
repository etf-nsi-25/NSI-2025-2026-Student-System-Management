using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;

namespace Faculty.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly FacultyDbContext _context;

        public CourseRepository(FacultyDbContext context)
        {
            _context = context;
        }

        public Task<Course> AddAsync(Course course)
        {
            course.Id = Guid.NewGuid();
            _context.Courses.Add(course);
            return Task.FromResult(course);
        }

        public Task<Course?> GetByIdAsync(Guid id)
            => Task.FromResult(_context.Courses.FirstOrDefault(x => x.Id == id));

        public Task<List<Course>> GetAllAsync()
            => Task.FromResult(_context.Courses.ToList());

        public Task<Course?> UpdateAsync(Course course)
        {
            var existing = _context.Courses.FirstOrDefault(x => x.Id == course.Id);
            if (existing == null)
                return Task.FromResult<Course?>(null);

            existing.Name = course.Name;
            existing.Code = course.Code;
            existing.Type = course.Type;
            existing.ProgramId = course.ProgramId;
            existing.ECTS = course.ECTS;

            return Task.FromResult(existing);
        }

        public Task<bool> DeleteAsync(Guid id)
        {
            var existing = _context.Courses.FirstOrDefault(x => x.Id == id);
            if (existing == null)
                return Task.FromResult(false);

            _context.Courses.Remove(existing);
            return Task.FromResult(true);
        }
    }
}
