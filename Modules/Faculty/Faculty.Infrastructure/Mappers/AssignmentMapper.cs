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
            UpdatedAt = persistence.UpdatedAt,
            StudentAssignments = new List<StudentAssignment>()
        };

        if (includeRelationships)
        {
            if (persistence.Course != null)
            {
                domain.Course = CourseMapper.ToDomain(persistence.Course, false);
            }

            if (persistence.StudentAssignments != null)
            {
                domain.StudentAssignments = persistence.StudentAssignments
                    .Select(ToDomainStudentAssignment)
                    .ToList();
            }
        }

        return domain;
    }

    private static StudentAssignment ToDomainStudentAssignment(StudentAssignmentSchema schema)
    {
        return new StudentAssignment
        {
            Id = schema.Id,
            FacultyId = schema.FacultyId,
            StudentId = schema.StudentId,
            AssignmentId = schema.AssignmentId,
            SubmissionDate = schema.SubmissionDate,
            Points = schema.Points,
            Grade = schema.Grade,
            Feedback = schema.Feedback,
            SubmissionUrl = schema.SubmissionUrl,
            CreatedAt = schema.CreatedAt,
            UpdatedAt = schema.UpdatedAt
        };
    }

    public static IEnumerable<AssignmentSchema> ToPersistenceCollection(IEnumerable<Assignment> domainCollection)
        => domainCollection?.Select(ToPersistence) ?? Enumerable.Empty<AssignmentSchema>();

    public static IEnumerable<Assignment> ToDomainCollection(IEnumerable<AssignmentSchema> persistenceCollection, bool includeRelationships = false)
        => persistenceCollection?.Select(p => ToDomain(p, includeRelationships)) ?? Enumerable.Empty<Assignment>();

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
        persistence.UpdatedAt = DateTime.UtcNow;
    }
}