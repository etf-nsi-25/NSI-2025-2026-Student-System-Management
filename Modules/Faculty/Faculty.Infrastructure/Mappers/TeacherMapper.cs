using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

/// <summary>
/// Maps between Teacher domain entities and persistence schema entities.
/// </summary>
public static class TeacherMapper
{
    public static TeacherSchema ToPersistence(Teacher domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new TeacherSchema
        {
            Id = domain.Id,
            FacultyId = domain.FacultyId,
            UserId = domain.UserId,
            Title = domain.Title,
            FirstName = domain.FirstName,
            LastName = domain.LastName,
            Office = domain.Office,
            Email = domain.Email,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Teacher ToDomain(TeacherSchema persistence, bool includeRelationships = false)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));

        var domain = new Teacher
        {
            Id = persistence.Id,
            FacultyId = persistence.FacultyId,
            UserId = persistence.UserId,
            Title = persistence.Title,
            FirstName = persistence.FirstName,
            LastName = persistence.LastName,
            Office = persistence.Office,
            Email = persistence.Email,
            CreatedAt = persistence.CreatedAt,
            UpdatedAt = persistence.UpdatedAt
        };

        if (includeRelationships && persistence.CourseAssignments != null)
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

        return domain;
    }

    public static IEnumerable<TeacherSchema> ToPersistenceCollection(IEnumerable<Teacher> domainCollection)
    {
        if (domainCollection == null)
            return Enumerable.Empty<TeacherSchema>();

        return domainCollection.Select(ToPersistence);
    }

    public static IEnumerable<Teacher> ToDomainCollection(IEnumerable<TeacherSchema> persistenceCollection, bool includeRelationships = false)
    {
        if (persistenceCollection == null)
            return Enumerable.Empty<Teacher>();

        return persistenceCollection.Select(p => ToDomain(p, includeRelationships));
    }

    public static void UpdatePersistence(TeacherSchema persistence, Teacher domain)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        persistence.UserId = domain.UserId;
        persistence.Title = domain.Title;
        persistence.FirstName = domain.FirstName;
        persistence.LastName = domain.LastName;
        persistence.Office = domain.Office;
        persistence.Email = domain.Email;
        persistence.UpdatedAt = domain.UpdatedAt ?? DateTime.UtcNow;
    }
}
