using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

/// <summary>
/// Maps between Course domain entities and persistence entities.
/// </summary>
public static class CourseMapper
{
    public static CourseSchema ToPersistence(Course domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new CourseSchema
        {
            Id = domain.Id,
            FacultyId = domain.FacultyId,
            Name = domain.Name,
            Code = domain.Code,
            Type = domain.Type,
            ProgramId = domain.ProgramId,
            ECTS = domain.ECTS,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Course ToDomain(CourseSchema persistence, bool includeRelationships = false)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));

        var domain = new Course
        {
            Id = persistence.Id,
            FacultyId = persistence.FacultyId,
            Name = persistence.Name,
            Code = persistence.Code,
            Type = persistence.Type,
            ProgramId = persistence.ProgramId,
            ECTS = persistence.ECTS,
            CreatedAt = persistence.CreatedAt,
            UpdatedAt = persistence.UpdatedAt
        };

        if (includeRelationships)
        {
            if (persistence.CourseAssignments != null)
            {
                domain.CourseAssignments = persistence.CourseAssignments
                    .Select(ca => new CourseAssignment
                    {
                        Id = ca.Id,
                        FacultyId = ca.FacultyId,
                        TeacherId = ca.TeacherId,
                        CourseId = ca.CourseId,
                        Role = ca.Role,
                        AcademicYearId = ca.AcademicYearId,
                        CreatedAt = ca.CreatedAt,
                        UpdatedAt = ca.UpdatedAt
                    })
                    .ToList();
            }
            else
            {
                domain.CourseAssignments = new List<CourseAssignment>();
            }

            if (persistence.Enrollments != null)
            {
                domain.Enrollments = persistence.Enrollments
                    .Select(e => new Enrollment
                    {
                        Id = e.Id,
                        FacultyId = e.FacultyId,
                        StudentId = e.StudentId,
                        CourseId = e.CourseId,
                        Status = e.Status,
                        Grade = e.Grade,
                        CreatedAt = e.CreatedAt,
                        UpdatedAt = e.UpdatedAt
                    })
                    .ToList();
            }
            else
            {
                domain.Enrollments = new List<Enrollment>();
            }

            if (persistence.Assignments != null)
            {
                domain.Assignments = persistence.Assignments
                    .Select(a => new Assignment
                    {
                        Id = a.Id,
                        FacultyId = a.FacultyId,
                        CourseId = a.CourseId,
                        Name = a.Name,
                        Description = a.Description,
                        DueDate = a.DueDate,
                        MaxPoints = a.MaxPoints,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    })
                    .ToList();
            }
            else
            {
                domain.Assignments = new List<Assignment>();
            }

            if (persistence.Exams != null)
            {
                domain.Exams = persistence.Exams
                    .Select(ex => new Exam
                    {
                        Id = ex.Id,
                        FacultyId = ex.FacultyId,
                        CourseId = ex.CourseId,
                        Name = ex.Name,
                        ExamDate = ex.ExamDate,
                        RegDeadline = ex.RegDeadline,
                        CreatedAt = ex.CreatedAt,
                        UpdatedAt = ex.UpdatedAt
                    })
                    .ToList();
            }
            else
            {
                domain.Exams = new List<Exam>();
            }

            if (persistence.Attendances != null)
            {
                domain.Attendances = persistence.Attendances
                    .Select(a => new Attendance
                    {
                        Id = a.Id,
                        FacultyId = a.FacultyId,
                        StudentId = a.StudentId,
                        CourseId = a.CourseId,
                        LectureDate = a.LectureDate,
                        Status = a.Status,
                        CreatedAt = a.CreatedAt,
                        UpdatedAt = a.UpdatedAt
                    })
                    .ToList();
            }
            else
            {
                domain.Attendances = new List<Attendance>();
            }
        }
        else
        {
            domain.CourseAssignments = new List<CourseAssignment>();
            domain.Enrollments = new List<Enrollment>();
            domain.Assignments = new List<Assignment>();
            domain.Exams = new List<Exam>();
            domain.Attendances = new List<Attendance>();
        }

        return domain;
    }

    public static IEnumerable<CourseSchema> ToPersistenceCollection(IEnumerable<Course> domainCollection)
    {
        if (domainCollection == null)
            return Enumerable.Empty<CourseSchema>();

        return domainCollection.Select(ToPersistence);
    }

    public static IEnumerable<Course> ToDomainCollection(IEnumerable<CourseSchema> persistenceCollection, bool includeRelationships = false)
    {
        if (persistenceCollection == null)
            return Enumerable.Empty<Course>();

        return persistenceCollection.Select(p => ToDomain(p, includeRelationships));
    }

    public static void UpdatePersistence(CourseSchema persistence, Course domain)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        persistence.Name = domain.Name;
        persistence.Code = domain.Code;
        persistence.Type = domain.Type;
        persistence.ProgramId = domain.ProgramId;
        persistence.ECTS = domain.ECTS;
        persistence.UpdatedAt = domain.UpdatedAt ?? DateTime.UtcNow;
    }
}
