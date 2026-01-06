using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

/// <summary>
/// Maps between Student domain entities and persistence entities.
/// </summary>
public static class StudentMapper
{
    public static StudentSchema ToPersistence(Student domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new StudentSchema
        {
            Id = domain.Id,
            FacultyId = domain.FacultyId,
            UserId = domain.UserId,
            IndexNumber = domain.IndexNumber,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            EnrollmentDate = domain.EnrollmentDate,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Student ToDomain(StudentSchema persistence, bool includeRelationships = false)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));

        var domain = new Student
        {
            Id = persistence.Id,
            FacultyId = persistence.FacultyId,
            UserId = persistence.UserId,
            IndexNumber = persistence.IndexNumber,
            FirstName = persistence.FirstName,
            LastName = persistence.LastName,
            EnrollmentDate = persistence.EnrollmentDate,
            CreatedAt = persistence.CreatedAt,
            UpdatedAt = persistence.UpdatedAt
        };

        if (includeRelationships)
        {
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

            if (persistence.StudentAssignments != null)
            {
                domain.StudentAssignments = persistence.StudentAssignments
                    .Select(sa => new StudentAssignment
                    {
                        Id = sa.Id,
                        FacultyId = sa.FacultyId,
                        StudentId = sa.StudentId,
                        AssignmentId = sa.AssignmentId,
                        SubmissionDate = sa.SubmissionDate,
                        Points = sa.Points,
                        Grade = sa.Grade,
                        Feedback = sa.Feedback,
                        SubmissionUrl = sa.SubmissionUrl,
                        CreatedAt = sa.CreatedAt,
                        UpdatedAt = sa.UpdatedAt
                    })
                    .ToList();
            }
            else
            {
                domain.StudentAssignments = new List<StudentAssignment>();
            }

            if (persistence.ExamRegistrations != null)
            {
                domain.ExamRegistrations = persistence.ExamRegistrations
                    .Select(er => new ExamRegistration
                    {
                        Id = er.Id,
                        FacultyId = er.FacultyId,
                        StudentId = er.StudentId,
                        ExamId = er.ExamId,
                        RegistrationDate = er.RegistrationDate,
                        Status = er.Status,
                        CreatedAt = er.CreatedAt,
                        UpdatedAt = er.UpdatedAt
                    })
                    .ToList();
            }
            else
            {
                domain.ExamRegistrations = new List<ExamRegistration>();
            }

            if (persistence.StudentExamGrades != null)
            {
                domain.StudentExamGrades = persistence.StudentExamGrades
                    .Select(seg => new StudentExamGrade
                    {
                        Id = seg.Id,
                        FacultyId = seg.FacultyId,
                        StudentId = seg.StudentId,
                        ExamId = seg.ExamId,
                        Passed = seg.Passed,
                        Points = seg.Points,
                        URL = seg.URL,
                        DateRecorded = seg.DateRecorded,
                        CreatedAt = seg.CreatedAt,
                        UpdatedAt = seg.UpdatedAt
                    })
                    .ToList();
            }
            else
            {
                domain.StudentExamGrades = new List<StudentExamGrade>();
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
            domain.Enrollments = new List<Enrollment>();
            domain.StudentAssignments = new List<StudentAssignment>();
            domain.ExamRegistrations = new List<ExamRegistration>();
            domain.StudentExamGrades = new List<StudentExamGrade>();
            domain.Attendances = new List<Attendance>();
        }

        return domain;
    }

    public static IEnumerable<StudentSchema> ToPersistenceCollection(IEnumerable<Student> domainCollection)
    {
        if (domainCollection == null)
            return Enumerable.Empty<StudentSchema>();

        return domainCollection.Select(ToPersistence);
    }

    public static IEnumerable<Student> ToDomainCollection(IEnumerable<StudentSchema> persistenceCollection, bool includeRelationships = false)
    {
        if (persistenceCollection == null)
            return Enumerable.Empty<Student>();

        return persistenceCollection.Select(p => ToDomain(p, includeRelationships));
    }


    public static void UpdatePersistence(StudentSchema persistence, Student domain)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        persistence.UserId = domain.UserId;
        persistence.IndexNumber = domain.IndexNumber;
        persistence.FirstName = domain.FirstName;
        persistence.LastName = domain.LastName;
        persistence.EnrollmentDate = domain.EnrollmentDate;
        persistence.UpdatedAt = domain.UpdatedAt ?? DateTime.UtcNow;
    }
}
