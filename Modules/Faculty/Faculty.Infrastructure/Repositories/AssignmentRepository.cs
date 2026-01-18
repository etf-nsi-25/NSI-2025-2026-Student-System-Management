using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Mappers;
using Faculty.Infrastructure.Schemas;
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

    public async Task<List<Assignment>> GetUpcomingByCourseIdsAsync(List<Guid> courseIds)
    {
        if (courseIds == null || courseIds.Count == 0)
        {
            return new List<Assignment>();
        }

        var now = DateTime.UtcNow;

        var assignmentSchemas = await _context.Assignments
            .Include(a => a.Course)
            .Where(a => courseIds.Contains(a.CourseId) &&
                       a.DueDate.HasValue &&
                       a.DueDate.Value > now)
            .OrderBy(a => a.DueDate)
            .ToListAsync();

        return AssignmentMapper.ToDomainCollection(assignmentSchemas, includeRelationships: true).ToList();
    }
}
