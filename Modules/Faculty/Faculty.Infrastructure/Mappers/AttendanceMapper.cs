using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

/// <summary>
/// Maps between Attendance domain entities and persistence entities.
/// </summary>
public static class AttendanceMapper
{
    public static AttendanceSchema ToPersistence(Attendance domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new AttendanceSchema
        {
            Id = domain.Id,
            FacultyId = domain.FacultyId,
            StudentId = domain.StudentId,
            CourseId = domain.CourseId,
            LectureDate = domain.LectureDate,
            Status = domain.Status,
            Note = domain.Note,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Attendance ToDomain(AttendanceSchema persistence, bool includeRelationships = false)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));

        var domain = new Attendance
        {
            Id = persistence.Id,
            FacultyId = persistence.FacultyId,
            StudentId = persistence.StudentId,
            CourseId = persistence.CourseId,
            LectureDate = persistence.LectureDate,
            Status = persistence.Status,
            Note = persistence.Note,
            CreatedAt = persistence.CreatedAt,
            UpdatedAt = persistence.UpdatedAt
        };

        if (includeRelationships)
        {
            if (persistence.Student != null)
            {
                domain.Student = StudentMapper.ToDomain(persistence.Student, false);
            }

            if (persistence.Course != null)
            {
                domain.Course = CourseMapper.ToDomain(persistence.Course, false);
            }
        }

        return domain;
    }

    public static IEnumerable<AttendanceSchema> ToPersistenceCollection(IEnumerable<Attendance> domainCollection)
    {
        if (domainCollection == null)
            return Enumerable.Empty<AttendanceSchema>();

        return domainCollection.Select(ToPersistence);
    }

    public static IEnumerable<Attendance> ToDomainCollection(IEnumerable<AttendanceSchema> persistenceCollection, bool includeRelationships = false)
    {
        if (persistenceCollection == null)
            return Enumerable.Empty<Attendance>();

        return persistenceCollection.Select(p => ToDomain(p, includeRelationships));
    }

    public static void UpdatePersistence(AttendanceSchema persistence, Attendance domain)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        persistence.StudentId = domain.StudentId;
        persistence.CourseId = domain.CourseId;
        persistence.LectureDate = domain.LectureDate;
        persistence.Status = domain.Status;
        persistence.Note = domain.Note;
        persistence.UpdatedAt = domain.UpdatedAt ?? DateTime.UtcNow;
    }
}

