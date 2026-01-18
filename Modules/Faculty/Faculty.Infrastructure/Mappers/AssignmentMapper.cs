using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

/// <summary>
/// Maps between Assignment domain entities and persistence entities.
/// </summary>
public static class AssignmentMapper
{
    public static AssignmentSchema ToPersistence(Assignment domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new AssignmentSchema
        {
            Id = domain.Id,
            FacultyId = domain.FacultyId,
            CourseId = domain.CourseId,
            Name = domain.Name,
            Description = domain.Description,
            DueDate = domain.DueDate,
            MaxPoints = domain.MaxPoints,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Assignment ToDomain(AssignmentSchema persistence, bool includeRelationships = false)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));

        var domain = new Assignment
        {
            Id = persistence.Id,
            FacultyId = persistence.FacultyId,
            CourseId = persistence.CourseId,
            Name = persistence.Name,
            Description = persistence.Description,
            DueDate = persistence.DueDate,
            MaxPoints = persistence.MaxPoints,
            CreatedAt = persistence.CreatedAt,
            UpdatedAt = persistence.UpdatedAt
        };

        if (includeRelationships)
        {
            if (persistence.Course != null)
            {
                domain.Course = CourseMapper.ToDomain(persistence.Course, false);
            }

            if (persistence.StudentAssignments != null)
            {
                domain.StudentAssignments = new List<StudentAssignment>();
            }
            else
            {
                domain.StudentAssignments = new List<StudentAssignment>();
            }
        }
        else
        {
            domain.StudentAssignments = new List<StudentAssignment>();
        }

        return domain;
    }

    public static IEnumerable<AssignmentSchema> ToPersistenceCollection(IEnumerable<Assignment> domainCollection)
    {
        if (domainCollection == null)
            return Enumerable.Empty<AssignmentSchema>();

        return domainCollection.Select(ToPersistence);
    }

    public static IEnumerable<Assignment> ToDomainCollection(IEnumerable<AssignmentSchema> persistenceCollection, bool includeRelationships = false)
    {
        if (persistenceCollection == null)
            return Enumerable.Empty<Assignment>();

        return persistenceCollection.Select(p => ToDomain(p, includeRelationships));
    }

    public static void UpdatePersistence(AssignmentSchema persistence, Assignment domain)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        persistence.CourseId = domain.CourseId;
        persistence.Name = domain.Name;
        persistence.Description = domain.Description;
        persistence.DueDate = domain.DueDate;
        persistence.MaxPoints = domain.MaxPoints;
        persistence.UpdatedAt = domain.UpdatedAt ?? DateTime.UtcNow;
    }
}
