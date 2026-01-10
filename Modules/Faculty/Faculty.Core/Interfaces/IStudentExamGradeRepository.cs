using Faculty.Core.Entities;

namespace Faculty.Core.Interfaces;

public interface IStudentExamGradeRepository
{
    Task<StudentExamGrade?> GetAsync(int studentId, int examId, CancellationToken ct);
    Task<IEnumerable<StudentExamGrade>> GetAllForExamAsync(int examId, CancellationToken ct);
    Task<StudentExamGrade> CreateAsync(StudentExamGrade grade, CancellationToken ct);
    Task<StudentExamGrade> UpdateAsync(StudentExamGrade grade, CancellationToken ct);
    Task DeleteAsync(StudentExamGrade grade, CancellationToken ct);
    Task<bool> ExamBelongsToFacultyAndTeacherAsync(int examId, Guid facultyId, int teacherId, CancellationToken ct);
}