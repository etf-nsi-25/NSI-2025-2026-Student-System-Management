namespace Faculty.Infrastructure.Repositories;

using Core.Entities;
using Core.Interfaces;
using Db;
using Microsoft.EntityFrameworkCore;

public class StudentExamGradeRepository : IStudentExamGradeRepository
{
    private readonly FacultyDbContext _context;

    public StudentExamGradeRepository(FacultyDbContext context)
    {
        _context = context;
    }

    public Task<StudentExamGrade?> GetAsync(
        int studentId,
        int examId,
        CancellationToken ct)
        => _context.StudentExamGrades
            .Include(g => g.Exam)
            .Include(g => g.Student)
            .FirstOrDefaultAsync(
                g => g.StudentId == studentId && g.ExamId == examId,
                ct);

    public async Task<StudentExamGrade> CreateAsync(
        StudentExamGrade grade,
        CancellationToken ct)
    {
        _context.StudentExamGrades.Add(grade);
        await _context.SaveChangesAsync(ct);
        return grade;
    }

    public async Task<StudentExamGrade> UpdateAsync(
        StudentExamGrade grade,
        CancellationToken ct)
    {
        _context.StudentExamGrades.Update(grade);
        await _context.SaveChangesAsync(ct);
        return grade;
    }

    public async Task DeleteAsync(
        StudentExamGrade grade,
        CancellationToken ct)
    {
        _context.StudentExamGrades.Remove(grade);
        await _context.SaveChangesAsync(ct);
    }

    public Task<bool> ExamBelongsToTeacherAsync(
        int examId,
        int teacherId,
        CancellationToken ct)
    {
        return _context.Exams
            .AnyAsync(
                e => e.Id == examId &&
                     e.Course.CourseAssignments.Any(ca => ca.TeacherId == teacherId),
                ct);
    }

    public async Task<IEnumerable<StudentExamGrade>> GetAllForExamAsync(
        int examId,
        CancellationToken ct)
    {
        return await _context.StudentExamGrades
            .Include(g => g.Student)
            .Where(g => g.ExamId == examId)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}