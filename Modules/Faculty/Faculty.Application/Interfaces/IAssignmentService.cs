using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

public interface IAssignmentService
{
    Task<List<StudentAssignmentOverviewDTO>> GetStudentAssignmentsForCourseAsync(Guid courseId, string userId);
}