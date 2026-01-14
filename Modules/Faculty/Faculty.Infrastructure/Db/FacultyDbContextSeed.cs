using Common.Core.Tenant;
using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Db;

public class FacultyDbContextSeed
{
    private readonly IScopedTenantContext _tenantContext;

    public FacultyDbContextSeed(IScopedTenantContext tenantContext)
    {
        _tenantContext = tenantContext;
    }

    public async Task SeedAsync(
        FacultyDbContext context,
        Guid teacherUserId,
        Guid studentUserId)
    {
        var tenantId = _tenantContext.CurrentTenantId()
            ?? throw new InvalidOperationException("Tenant not set");

        if (!await context.Teachers
                .AnyAsync(t => t.FacultyId == tenantId))
        {
            context.Teachers.Add(new TeacherSchema
            {
                FacultyId = tenantId,
                UserId = teacherUserId.ToString(),
                Title = "Dr",
                FirstName = "Emir",
                LastName = "Buza",
                Email = "emir.buza@unsa.ba",
                Office = "A-203",
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        if (!await context.Students
                .AnyAsync(s => s.FacultyId == tenantId))
        {
            context.Students.Add(new StudentSchema
            {
                FacultyId = tenantId,
                UserId = studentUserId.ToString(),
                IndexNumber = "IB20001",
                FirstName = "Niko",
                LastName = "Nikic",
                EnrollmentDate = DateTime.UtcNow.AddYears(-2),
                CreatedAt = DateTime.UtcNow
            });

            await context.SaveChangesAsync();
        }

        if (!await context.Courses
                .AnyAsync(c => c.FacultyId == tenantId))
        {
            context.Courses.AddRange(
                new CourseSchema
                {
                    Id = Guid.NewGuid(),
                    FacultyId = tenantId,
                    Name = "Software Engineering",
                    Code = "SE101",
                    ECTS = 6,
                    Type = CourseType.Mandatory,
                    CreatedAt = DateTime.UtcNow
                },
                new CourseSchema
                {
                    Id = Guid.NewGuid(),
                    FacultyId = tenantId,
                    Name = "Databases",
                    Code = "DB202",
                    ECTS = 5,
                    Type = CourseType.Mandatory,
                    CreatedAt = DateTime.UtcNow
                }
            );

            await context.SaveChangesAsync();
        }

        var teacher = await context.Teachers
            .IgnoreQueryFilters()
            .FirstAsync(t => t.FacultyId == tenantId);

        var students = await context.Students
            .IgnoreQueryFilters()
            .Where(s => s.FacultyId == tenantId)
            .ToListAsync();

        var courses = await context.Courses
            .IgnoreQueryFilters()
            .Where(c => c.FacultyId == tenantId)
            .ToListAsync();

        foreach (var course in courses)
        {
            if (!await context.CourseAssignments
                    .IgnoreQueryFilters()
                    .AnyAsync(ca =>
                        ca.CourseId == course.Id &&
                        ca.TeacherId == teacher.Id))
            {
                context.CourseAssignments.Add(new CourseAssignmentSchema
                {
                    FacultyId = tenantId,
                    CourseId = course.Id,
                    TeacherId = teacher.Id,
                    Role = "Lecturer",
                    AcademicYearId = 2024,
                    CreatedAt = DateTime.UtcNow
                });
            }
        }

        await context.SaveChangesAsync();

        foreach (var student in students)
        {
            foreach (var course in courses)
            {
                if (!await context.Enrollments
                        .IgnoreQueryFilters()
                        .AnyAsync(e =>
                            e.StudentId == student.Id &&
                            e.CourseId == course.Id))
                {
                    context.Enrollments.Add(new EnrollmentSchema
                    {
                        FacultyId = tenantId,
                        StudentId = student.Id,
                        CourseId = course.Id,
                        Status = "Enrolled",
                        CreatedAt = DateTime.UtcNow
                    });
                }
            }
        }

        await context.SaveChangesAsync();
    }
}