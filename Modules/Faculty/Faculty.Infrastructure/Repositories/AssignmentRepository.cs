using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Mappers;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories;

/// <summary>
/// Repository implementation for Assignment operations.
/// </summary>
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

        return AssignmentMapper
            .ToDomainCollection(schemas, includeRelationships: true)
            .ToList();
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

    public async Task<List<Assignment>> GetUpcomingByCourseIdsAsync(List<Guid> courseIds)
    {
        if (courseIds == null || courseIds.Count == 0)
        {
            return new List<Assignment>();
        }

        var now = DateTime.UtcNow;

        var assignmentSchemas = await _context.Assignments
            .Include(a => a.Course)
            .Where(a =>
                courseIds.Contains(a.CourseId) &&
                a.DueDate.HasValue &&
                a.DueDate.Value > now)
            .OrderBy(a => a.DueDate)
            .ToListAsync();

        return AssignmentMapper
            .ToDomainCollection(assignmentSchemas, includeRelationships: true)
            .ToList();
    }
}