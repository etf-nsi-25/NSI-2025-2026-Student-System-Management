using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces;

public interface IStudentExamRegistrationRepository
{
    Task<Student?> GetStudentByUserIdAsync(string userId, CancellationToken cancellationToken = default);
    Task<Exam?> GetExamWithDetailsAsync(int examId, CancellationToken cancellationToken = default);
    Task<bool> HasExistingRegistrationAsync(int studentId, int examId, CancellationToken cancellationToken = default);
    Task<ExamRegistration> SaveRegistrationAsync(ExamRegistration registration, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Exam>> GetEligibleExamsAsync(
        int studentId,
        IReadOnlyCollection<Guid> enrolledCourseIds,
        DateTime currentUtc,
        CancellationToken cancellationToken = default);
    Task<IReadOnlyList<ExamRegistration>> GetRegistrationsAsync(int studentId, CancellationToken cancellationToken = default);
}
