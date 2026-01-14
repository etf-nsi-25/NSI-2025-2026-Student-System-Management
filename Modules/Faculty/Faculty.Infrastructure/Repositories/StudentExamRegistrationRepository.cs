using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Faculty.Infrastructure.Mappers;
using Faculty.Infrastructure.Schemas;
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
        var studentSchema = await _context.Students
            .Include(s => s.Enrollments)
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

        return studentSchema != null 
            ? StudentMapper.ToDomain(studentSchema, includeRelationships: true) 
            : null;
    }

    public async Task<Exam?> GetExamWithDetailsAsync(int examId, CancellationToken cancellationToken = default)
    {
        var examSchema = await _context.Exams
            .Include(e => e.Course)
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == examId, cancellationToken);

        return examSchema != null 
            ? ExamMapper.ToDomain(examSchema, includeRelationships: true) 
            : null;
    }

    public Task<bool> HasExistingRegistrationAsync(int studentId, int examId, CancellationToken cancellationToken = default)
    {
        return _context.ExamRegistrations.AnyAsync(r => r.StudentId == studentId && r.ExamId == examId, cancellationToken);
    }

    public async Task<ExamRegistration> SaveRegistrationAsync(ExamRegistration registration, CancellationToken cancellationToken = default)
    {
        registration.CreatedAt = DateTime.UtcNow;
        var registrationSchema = ExamRegistrationMapper.ToPersistence(registration);
        await _context.ExamRegistrations.AddAsync(registrationSchema, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return ExamRegistrationMapper.ToDomain(registrationSchema, includeRelationships: false);
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

        var examSchemas = await query
            .OrderBy(e => e.ExamDate)
            .ToListAsync(cancellationToken);

        return ExamMapper.ToDomainCollection(examSchemas, includeRelationships: true).ToList();
    }

    public async Task<IReadOnlyList<ExamRegistration>> GetRegistrationsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        var registrationSchemas = await _context.ExamRegistrations
            .Where(r => r.StudentId == studentId)
            .Include(r => r.Exam)
                .ThenInclude(e => e.Course)
            .AsNoTracking()
            .OrderByDescending(r => r.RegistrationDate)
            .ToListAsync(cancellationToken);

        return ExamRegistrationMapper.ToDomainCollection(registrationSchemas, includeRelationships: true).ToList();
    }
}
