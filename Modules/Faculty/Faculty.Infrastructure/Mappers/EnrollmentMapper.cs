using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

/// <summary>
/// Maps between Enrollment domain entities and persistence entities.
/// </summary>
public static class EnrollmentMapper
{
    public static EnrollmentSchema ToPersistence(Enrollment domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new EnrollmentSchema
        {
            Id = domain.Id,
            FacultyId = domain.FacultyId,
            StudentId = domain.StudentId,
            CourseId = domain.CourseId,
            Status = domain.Status,
            Grade = domain.Grade,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Enrollment ToDomain(EnrollmentSchema persistence, bool includeRelationships = false)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));

        var domain = new Enrollment
        {
            Id = persistence.Id,
            FacultyId = persistence.FacultyId,
            StudentId = persistence.StudentId,
            CourseId = persistence.CourseId,
            Status = persistence.Status,
            Grade = persistence.Grade,
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

    public static IEnumerable<EnrollmentSchema> ToPersistenceCollection(IEnumerable<Enrollment> domainCollection)
    {
        if (domainCollection == null)
            return Enumerable.Empty<EnrollmentSchema>();

        return domainCollection.Select(ToPersistence);
    }

    public static IEnumerable<Enrollment> ToDomainCollection(IEnumerable<EnrollmentSchema> persistenceCollection, bool includeRelationships = false)
    {
        if (persistenceCollection == null)
            return Enumerable.Empty<Enrollment>();

        return persistenceCollection.Select(p => ToDomain(p, includeRelationships));
    }

    public static void UpdatePersistence(EnrollmentSchema persistence, Enrollment domain)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        persistence.StudentId = domain.StudentId;
        persistence.CourseId = domain.CourseId;
        persistence.Status = domain.Status;
        persistence.Grade = domain.Grade;
        persistence.UpdatedAt = domain.UpdatedAt ?? DateTime.UtcNow;
    }
}

