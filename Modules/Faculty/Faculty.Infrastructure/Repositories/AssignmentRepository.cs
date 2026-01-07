using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Mappers; 
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly FacultyDbContext _context;

    public AssignmentRepository(FacultyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<List<Assignment>> GetAssignmentsWithSubmissionsAsync(Guid courseId, int studentId)
    {
        var schemas = await _context.Assignments
            .Where(a => a.CourseId == courseId)
            .Include(a => a.StudentAssignments.Where(sa => sa.StudentId == studentId))
            .OrderBy(a => a.DueDate)
            .ToListAsync();

        return AssignmentMapper.ToDomainCollection(schemas, includeRelationships: true);
    }

    public async Task<bool> IsStudentEnrolledAsync(int studentId, Guid courseId)
    {
        return await _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId);
    }

    public async Task<Guid> GetCourseFacultyIdAsync(Guid courseId)
    {
        var course = await _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId);
            
        return course?.FacultyId ?? Guid.Empty;
    }
}