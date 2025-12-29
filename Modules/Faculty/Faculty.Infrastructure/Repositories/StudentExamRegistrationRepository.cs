using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories;

public class StudentExamRegistrationRepository : IStudentExamRegistrationRepository
{
    private readonly FacultyDbContext _context;

    public StudentExamRegistrationRepository(FacultyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Student?> GetStudentByUserIdAsync(string userId, CancellationToken cancellationToken = default)
    {
        return await _context.Students
            .Include(s => s.Enrollments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);
    }

    public async Task<Exam?> GetExamWithDetailsAsync(int examId, CancellationToken cancellationToken = default)
    {
        return await _context.Exams
            .Include(e => e.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == examId, cancellationToken);
    }

    public Task<bool> HasExistingRegistrationAsync(int studentId, int examId, CancellationToken cancellationToken = default)
    {
        return _context.ExamRegistrations.AnyAsync(r => r.StudentId == studentId && r.ExamId == examId, cancellationToken);
    }

    public async Task<ExamRegistration> SaveRegistrationAsync(ExamRegistration registration, CancellationToken cancellationToken = default)
    {
        await _context.ExamRegistrations.AddAsync(registration, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return registration;
    }

    public async Task<IReadOnlyList<Exam>> GetEligibleExamsAsync(
        int studentId,
        IReadOnlyCollection<Guid> enrolledCourseIds,
        DateTime currentUtc,
        CancellationToken cancellationToken = default)
    {
        if (enrolledCourseIds.Count == 0)
        {
            return Array.Empty<Exam>();
        }

        var registeredExamIds = await _context.ExamRegistrations
            .Where(r => r.StudentId == studentId)
            .Select(r => r.ExamId)
            .ToListAsync(cancellationToken);

        var query = _context.Exams
            .Include(e => e.Course)
            .AsNoTracking()
            .Where(e => enrolledCourseIds.Contains(e.CourseId))
            .Where(e => e.ExamDate.HasValue && e.ExamDate.Value > currentUtc)
            .Where(e => !e.RegDeadline.HasValue || e.RegDeadline.Value >= currentUtc);

        if (registeredExamIds.Count > 0)
        {
            query = query.Where(e => !registeredExamIds.Contains(e.Id));
        }

        return await query
            .OrderBy(e => e.ExamDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<ExamRegistration>> GetRegistrationsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _context.ExamRegistrations
            .Where(r => r.StudentId == studentId)
            .Include(r => r.Exam)
                .ThenInclude(e => e.Course)
            .AsNoTracking()
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync(cancellationToken);
    }
}
