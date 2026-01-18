using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces;

public interface IAssignmentRepository
{
    Task<List<Assignment>> GetAssignmentsWithSubmissionsAsync(Guid courseId, int studentId);

    Task<bool> IsStudentEnrolledAsync(int studentId, Guid courseId);

    Task<Guid> GetCourseFacultyIdAsync(Guid courseId);

    Task<List<Assignment>> GetUpcomingByCourseIdsAsync(List<Guid> courseIds);
}