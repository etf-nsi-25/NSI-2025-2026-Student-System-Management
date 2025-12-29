using Faculty.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace Faculty.Infrastructure.Db;

public static class FacultyDbContextSeed
{
    public static async Task SeedAsync(
        FacultyDbContext context,
        Guid facultyId,
        Guid teacherUserId,
        Guid studentUserId)
    {
        if (!await context.Teachers
                .IgnoreQueryFilters()
                .AnyAsync(t => t.FacultyId == facultyId))
        {
            context.Teachers.Add(new Teacher
            {
                FacultyId = facultyId,
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
                .IgnoreQueryFilters()
                .AnyAsync(s => s.FacultyId == facultyId))
        {
            context.Students.Add(new Student
            {
                FacultyId = facultyId,
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
                .IgnoreQueryFilters()
                .AnyAsync(c => c.FacultyId == facultyId))
        {
            context.Courses.AddRange(
                new Course
                {
                    Id = Guid.NewGuid(),
                    FacultyId = facultyId,
                    Name = "Software Engineering",
                    Code = "SE101",
                    ECTS = 6,
                    Type = CourseType.Mandatory,
                    CreatedAt = DateTime.UtcNow
                },
                new Course
                {
                    Id = Guid.NewGuid(),
                    FacultyId = facultyId,
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
            .FirstAsync(t => t.FacultyId == facultyId);

        var students = await context.Students
            .IgnoreQueryFilters()
            .Where(s => s.FacultyId == facultyId)
            .ToListAsync();

        var courses = await context.Courses
            .IgnoreQueryFilters()
            .Where(c => c.FacultyId == facultyId)
            .ToListAsync();

        foreach (var course in courses)
        {
            if (!await context.CourseAssignments
                    .IgnoreQueryFilters()
                    .AnyAsync(ca =>
                        ca.CourseId == course.Id &&
                        ca.TeacherId == teacher.Id))
            {
                context.CourseAssignments.Add(new CourseAssignment
                {
                    FacultyId = facultyId,
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
                    context.Enrollments.Add(new Enrollment
                    {
                        FacultyId = facultyId,
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