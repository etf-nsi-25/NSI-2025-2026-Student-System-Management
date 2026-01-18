using Faculty.Application.DTOs;
using Faculty.Application.Interfaces;
using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Microsoft.Extensions.Logging;

namespace Faculty.Application.Services;

public class StudentExamGradeService : IStudentExamGradeService
{
    private readonly IStudentExamGradeRepository _repo;
    private readonly ILogger<StudentExamGradeService> _logger;

    public StudentExamGradeService(
        IStudentExamGradeRepository repo,
        ILogger<StudentExamGradeService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<StudentGradeListResponseDTO> GetAllForExamAsync(
        int examId,
        int teacherId,
        CancellationToken ct)
    {
        if (!await _repo.ExamBelongsToTeacherAsync(examId, teacherId, ct))
        {
            _logger.LogWarning(
                "Unauthorized GET attempt for ExamId={ExamId} by TeacherId={TeacherId}",
                examId,
                teacherId);

            throw new UnauthorizedAccessException("You can view only your own exams.");
        }

        var grades = (await _repo.GetAllForExamAsync(examId, ct)).ToList();

        _logger.LogInformation(
            "Fetched {Count} grades for ExamId={ExamId} by TeacherId={TeacherId}",
            grades.Count,
            examId,
            teacherId);

        return new StudentGradeListResponseDTO
        {
            ExamId = examId,
            ExamName = grades.FirstOrDefault()?.Exam?.Name ?? "Unknown",
            Grades = grades.Select(g => new GradeResponseDTO
            {
                StudentId = g.StudentId,
                StudentName = $"{g.Student.FirstName} {g.Student.LastName}",
                Points = g.Points,
                Passed = g.Passed,
                DateRecorded = g.DateRecorded,
                Url = g.URL
            })
        };
    }

    public async Task<GradeResponseDTO> CreateOrUpdateAsync(
        int examId,
        int studentId,
        GradeRequestDTO requestDto,
        int teacherId,
        CancellationToken ct)
    {
        if (!await _repo.ExamBelongsToTeacherAsync(examId, teacherId, ct))
        {
            _logger.LogWarning(
                "Unauthorized POST attempt for ExamId={ExamId} StudentId={StudentId} by TeacherId={TeacherId}",
                examId,
                studentId,
                teacherId);

            throw new UnauthorizedAccessException("You can grade only your own exams.");
        }

        if (requestDto.Points < 0 || requestDto.Points > 100)
        {
            _logger.LogWarning(
                "Invalid points: {Points} for StudentId={StudentId} ExamId={ExamId}",
                requestDto.Points,
                studentId,
                examId);

            throw new InvalidOperationException("Points must be between 0 and 100.");
        }

        var grade = await _repo.GetAsync(studentId, examId, ct);

        if (grade == null)
        {
            grade = new StudentExamGrade
            {
                StudentId = studentId,
                ExamId = examId,
                Points = requestDto.Points,
                Passed = requestDto.Passed,
                URL = requestDto.Url,
                DateRecorded = DateTime.UtcNow
            };

            await _repo.CreateAsync(grade, ct);

            _logger.LogInformation(
                "Created grade for StudentId={StudentId} ExamId={ExamId} by TeacherId={TeacherId}",
                studentId,
                examId,
                teacherId);
        }
        else
        {
            grade.Points = requestDto.Points;
            grade.Passed = requestDto.Passed;
            grade.URL = requestDto.Url;
            grade.DateRecorded = DateTime.UtcNow;

            await _repo.UpdateAsync(grade, ct);

            _logger.LogInformation(
                "Updated grade for StudentId={StudentId} ExamId={ExamId} by TeacherId={TeacherId}",
                studentId,
                examId,
                teacherId);
        }

        return new GradeResponseDTO
        {
            StudentId = grade.StudentId,
            StudentName = $"{grade.Student.FirstName} {grade.Student.LastName}",
            Points = grade.Points,
            Passed = grade.Passed,
            DateRecorded = grade.DateRecorded,
            Url = grade.URL
        };
    }

    public async Task<GradeResponseDTO> UpdateAsync(
        int studentId,
        int examId,
        GradeUpdateRequestDTO requestDto,
        int teacherId,
        CancellationToken ct)
    {
        if (!await _repo.ExamBelongsToTeacherAsync(examId, teacherId, ct))
        {
            _logger.LogWarning(
                "Unauthorized PUT attempt for ExamId={ExamId} StudentId={StudentId} by TeacherId={TeacherId}",
                examId,
                studentId,
                teacherId);

            throw new UnauthorizedAccessException("You can update only your own exams.");
        }

        var grade = await _repo.GetAsync(studentId, examId, ct)
            ?? throw new KeyNotFoundException(
                $"Grade not found for StudentId={studentId} ExamId={examId}");

        if (requestDto.Points.HasValue)
        {
            if (requestDto.Points is < 0 or > 100)
            {
                _logger.LogWarning(
                    "Invalid points update: {Points} for StudentId={StudentId} ExamId={ExamId}",
                    requestDto.Points,
                    studentId,
                    examId);

                throw new InvalidOperationException("Points must be between 0 and 100.");
            }

            grade.Points = requestDto.Points.Value;
            grade.Passed = grade.Points >= 55;
        }

        if (requestDto.Passed.HasValue)
            grade.Passed = requestDto.Passed.Value;

        if (requestDto.DateRecorded.HasValue)
            grade.DateRecorded = requestDto.DateRecorded.Value;

        await _repo.UpdateAsync(grade, ct);

        _logger.LogInformation(
            "Updated grade for StudentId={StudentId} ExamId={ExamId} by TeacherId={TeacherId}",
            studentId,
            examId,
            teacherId);

        return new GradeResponseDTO
        {
            StudentId = grade.StudentId,
            StudentName = $"{grade.Student.FirstName} {grade.Student.LastName}",
            Points = grade.Points,
            Passed = grade.Passed,
            DateRecorded = grade.DateRecorded,
            Url = grade.URL
        };
    }

    public async Task DeleteAsync(
        int studentId,
        int examId,
        int teacherId,
        CancellationToken ct)
    {
        if (!await _repo.ExamBelongsToTeacherAsync(examId, teacherId, ct))
        {
            _logger.LogWarning(
                "Unauthorized DELETE attempt for ExamId={ExamId} StudentId={StudentId} by TeacherId={TeacherId}",
                examId,
                studentId,
                teacherId);

            throw new UnauthorizedAccessException("You can delete only your own exams.");
        }

        var grade = await _repo.GetAsync(studentId, examId, ct)
            ?? throw new KeyNotFoundException(
                $"Grade not found for StudentId={studentId} ExamId={examId}");

        await _repo.DeleteAsync(grade, ct);

        _logger.LogInformation(
            "Deleted grade for StudentId={StudentId} ExamId={ExamId} by TeacherId={TeacherId}",
            studentId,
            examId,
            teacherId);
    }
}
