using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

public interface IStudentExamGradeService
{
    Task<StudentGradeListResponse> GetAllForExamAsync(int examId, Guid facultyId, int teacherId, CancellationToken ct);
    Task<GradeResponse> CreateOrUpdateAsync(GradeRequest request, Guid facultyId, int teacherId, CancellationToken ct);
    Task<GradeResponse> UpdateAsync(int studentId, int examId, GradeUpdateRequest request, Guid facultyId,
        int teacherId, CancellationToken ct);
    Task DeleteAsync(int studentId, int examId, Guid facultyId, int teacherId, CancellationToken ct);
}
