using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            return await _context.CourseAssignments
                .Where(ca => ca.CourseId == courseId)
                .Include(ca => ca.Teacher)
                .Select(ca => ca.Teacher)
                .FirstOrDefaultAsync();
        }
    }
}
