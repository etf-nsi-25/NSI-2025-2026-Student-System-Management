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

        if (await context.Students.CountAsync() < 2)
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

            context.Students.AddRange(
                new StudentSchema { FacultyId = tenantId, UserId = Guid.NewGuid().ToString(), IndexNumber = "IB20002", FirstName = "Amra", LastName = "Hadžić", EnrollmentDate = DateTime.UtcNow.AddYears(-2), CreatedAt = DateTime.UtcNow },
                new StudentSchema { FacultyId = tenantId, UserId = Guid.NewGuid().ToString(), IndexNumber = "IB20003", FirstName = "Kenan", LastName = "Suljić", EnrollmentDate = DateTime.UtcNow.AddYears(-1), CreatedAt = DateTime.UtcNow },
                new StudentSchema { FacultyId = tenantId, UserId = Guid.NewGuid().ToString(), IndexNumber = "IB20004", FirstName = "Lejla", LastName = "Kadić", EnrollmentDate = DateTime.UtcNow.AddYears(-2), CreatedAt = DateTime.UtcNow },
                new StudentSchema { FacultyId = tenantId, UserId = Guid.NewGuid().ToString(), IndexNumber = "IB20005", FirstName = "Tarik", LastName = "Memić", EnrollmentDate = DateTime.UtcNow.AddYears(-1), CreatedAt = DateTime.UtcNow },
                new StudentSchema { FacultyId = tenantId, UserId = Guid.NewGuid().ToString(), IndexNumber = "IB20006", FirstName = "Emina", LastName = "Bešić", EnrollmentDate = DateTime.UtcNow.AddYears(-2), CreatedAt = DateTime.UtcNow }
            );

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

        var random = new Random();
        for (int i = 0; i < students.Count; i++) // Koristimo for petlju da imamo pristup indeksu 'i'
        {
            var student = students[i];
            foreach (var course in courses)
            {
                var enrollment = await context.Enrollments
                        .IgnoreQueryFilters()
                        .FirstOrDefaultAsync(e => 
                            e.StudentId == student.Id && 
                            e.CourseId == course.Id);

                // Odredimo ocjenu: ako je npr. drugi student (i == 1), daj mu namjerno malo bodova
                int generatedGrade = (i == 1 || i == 4) ? random.Next(20, 50) : random.Next(60, 100);

                if (enrollment == null)
                {
                    enrollment = new EnrollmentSchema
                    {
                        FacultyId = tenantId,
                        StudentId = student.Id,
                        CourseId = course.Id,
                        Status = "Enrolled",
                        Grade = generatedGrade,
                        CreatedAt = DateTime.UtcNow
                    };
                    context.Enrollments.Add(enrollment);
                }
                else
                {
                    enrollment.Grade = generatedGrade;
                    enrollment.Status = generatedGrade < 55 ? "Failed" : "Completed";
                    context.Enrollments.Update(enrollment);
                }

                if (!await context.Attendances
                        .IgnoreQueryFilters()
                        .AnyAsync(a => a.StudentId == student.Id && a.CourseId == course.Id))
                {
                    for (int j = 1; j <= 5; j++)
                    {
                        context.Attendances.Add(new AttendanceSchema
                        {
                            FacultyId = tenantId,
                            StudentId = student.Id,
                            CourseId = course.Id,
                            LectureDate = DateTime.UtcNow.AddDays(-j * 7),
                            Status = random.Next(0, 10) < 8 ? "Present" : "Absent",
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }
            }
        }

        await context.SaveChangesAsync();
    }
}