using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories
{
    public class CourseAssignmentRepository : ICourseAssignmentRepository
    {
        private readonly FacultyDbContext _context;

        public CourseAssignmentRepository(FacultyDbContext context)
        {
            _context = context;
        }

        public async Task<Teacher?> GetTeacherForCourseAsync(Guid courseId)
        {
            var teacherSchema = await _context.CourseAssignments
                .AsNoTracking()
                .Where(ca => ca.CourseId == courseId)
                .Include(ca => ca.Teacher)
                .Select(ca => ca.Teacher)
                .FirstOrDefaultAsync();

            return teacherSchema == null
                ? null
                : TeacherMapper.ToDomain(teacherSchema, includeRelationships: false);
        }
    }
}
