using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces;

public interface IAssignmentRepository
{
    Task<List<Assignment>> GetUpcomingByCourseIdsAsync(List<Guid> courseIds);
}
