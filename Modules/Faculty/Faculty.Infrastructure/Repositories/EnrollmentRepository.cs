using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Faculty.Core.Entities;
using Faculty.Core.Interfaces;
using Faculty.Infrastructure.Db;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Repositories;

public class EnrollmentRepository : IEnrollmentRepository
{
    private readonly FacultyDbContext _context;

    public EnrollmentRepository(FacultyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public Task<Student?> GetStudentByUserIdAsync(string userId, CancellationToken cancellationToken = default)
        => _context.Students
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId, cancellationToken);

    public Task<Course?> GetCourseAsync(Guid courseId, CancellationToken cancellationToken = default)
        => _context.Courses
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == courseId, cancellationToken);

    public Task<bool> IsAlreadyEnrolledAsync(int studentId, Guid courseId, CancellationToken cancellationToken = default)
        => _context.Enrollments
            .AnyAsync(e => e.StudentId == studentId && e.CourseId == courseId, cancellationToken);

    public async Task<Enrollment> CreateEnrollmentAsync(Enrollment enrollment, CancellationToken cancellationToken = default)
    {
        await _context.Enrollments.AddAsync(enrollment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return enrollment;
    }
}

