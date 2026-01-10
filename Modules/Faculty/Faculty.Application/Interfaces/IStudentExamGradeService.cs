using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

public interface IStudentExamGradeService
{
    Task<StudentGradeListResponseDTO> GetAllForExamAsync(int examId, Guid facultyId, int teacherId, CancellationToken ct);
    Task<GradeResponseDTO> CreateOrUpdateAsync(GradeRequestDTO requestDto, Guid facultyId, int teacherId, CancellationToken ct);
    Task<GradeResponseDTO> UpdateAsync(int studentId, int examId, GradeUpdateRequestDTO requestDto, Guid facultyId,
        int teacherId, CancellationToken ct);
    Task DeleteAsync(int studentId, int examId, Guid facultyId, int teacherId, CancellationToken ct);
}
