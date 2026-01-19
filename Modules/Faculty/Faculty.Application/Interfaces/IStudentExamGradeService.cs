using Faculty.Application.DTOs;

namespace Faculty.Application.Interfaces;

public interface IStudentExamGradeService
{
    Task<StudentGradeListResponseDTO> GetAllForExamAsync(
        int examId,
        int teacherId,
        CancellationToken ct);

    Task<GradeResponseDTO> CreateAsync(
        int examId,
        int studentId,
        GradeRequestDTO requestDto,
        int teacherId,
        CancellationToken ct);

    Task<GradeResponseDTO> UpdateAsync(
        int studentId,
        int examId,
        GradeUpdateRequestDTO requestDto,
        int teacherId,
        CancellationToken ct);

    Task DeleteAsync(
        int studentId,
        int examId,
        int teacherId,
        CancellationToken ct);
}
