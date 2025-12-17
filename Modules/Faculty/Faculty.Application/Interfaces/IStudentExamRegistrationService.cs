using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

public interface IStudentExamRegistrationService
{
    Task<ExamRegistrationResponseDto> RegisterAsync(int examId, string userId, Guid facultyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<AvailableExamDto>> GetAvailableExamsAsync(string userId, Guid facultyId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<RegisteredExamDto>> GetRegistrationsAsync(string userId, Guid facultyId, CancellationToken cancellationToken = default);
}
