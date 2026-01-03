using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories
{
    public class CourseRepository : ICourseRepository
    {
        private readonly FacultyDbContext _context;

        public CourseRepository(FacultyDbContext context)
        {
            _context = context;
        }

        public async Task<Course> AddAsync(Course course, CancellationToken cancellationToken = default)
        {
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
