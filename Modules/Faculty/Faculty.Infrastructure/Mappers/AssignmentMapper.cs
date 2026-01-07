using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

public static class AssignmentMapper
{
    public static Assignment ToDomain(AssignmentSchema schema, bool includeRelationships = false)
    {
        if (schema == null) return null!;

        var domain = new Assignment
        {
            Id = schema.Id,
            FacultyId = schema.FacultyId,
            CourseId = schema.CourseId,
            Name = schema.Name,
            Description = schema.Description,
            DueDate = schema.DueDate,
            MaxPoints = schema.MaxPoints,
            CreatedAt = schema.CreatedAt,
            UpdatedAt = schema.UpdatedAt
        };

        if (includeRelationships && schema.StudentAssignments != null)
        {
            domain.StudentAssignments = schema.StudentAssignments
                .Select(sa => ToDomainStudentAssignment(sa))
                .ToList();
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

    public static List<Assignment> ToDomainCollection(IEnumerable<AssignmentSchema> schemas, bool includeRelationships = false)
    {
        return schemas.Select(s => ToDomain(s, includeRelationships)).ToList();
    }
}