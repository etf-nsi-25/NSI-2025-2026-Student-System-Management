using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

/// <summary>
/// Maps between ExamRegistration domain entities and persistence entities.
/// </summary>
public static class ExamRegistrationMapper
{
    public static ExamRegistrationSchema ToPersistence(ExamRegistration domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new ExamRegistrationSchema
        {
            Id = domain.Id,
            FacultyId = domain.FacultyId,
            StudentId = domain.StudentId,
            ExamId = domain.ExamId,
            RegistrationDate = domain.RegistrationDate,
            Status = domain.Status,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static ExamRegistration ToDomain(ExamRegistrationSchema persistence, bool includeRelationships = false)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));

        var domain = new ExamRegistration
        {
            Id = persistence.Id,
            FacultyId = persistence.FacultyId,
            StudentId = persistence.StudentId,
            ExamId = persistence.ExamId,
            RegistrationDate = persistence.RegistrationDate,
            Status = persistence.Status,
            CreatedAt = persistence.CreatedAt,
            UpdatedAt = persistence.UpdatedAt
        };

        if (includeRelationships)
        {
            if (persistence.Student != null)
            {
                domain.Student = StudentMapper.ToDomain(persistence.Student, false);
            }

            if (persistence.Exam != null)
            {
                domain.Exam = ExamMapper.ToDomain(persistence.Exam, false);
            }
        }

        return domain;
    }

    public static IEnumerable<ExamRegistrationSchema> ToPersistenceCollection(IEnumerable<ExamRegistration> domainCollection)
    {
        if (domainCollection == null)
            return Enumerable.Empty<ExamRegistrationSchema>();

        return domainCollection.Select(ToPersistence);
    }

    public static IEnumerable<ExamRegistration> ToDomainCollection(IEnumerable<ExamRegistrationSchema> persistenceCollection, bool includeRelationships = false)
    {
        if (persistenceCollection == null)
            return Enumerable.Empty<ExamRegistration>();

        return persistenceCollection.Select(p => ToDomain(p, includeRelationships));
    }

    public static void UpdatePersistence(ExamRegistrationSchema persistence, ExamRegistration domain)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        persistence.StudentId = domain.StudentId;
        persistence.ExamId = domain.ExamId;
        persistence.RegistrationDate = domain.RegistrationDate;
        persistence.Status = domain.Status;
        persistence.UpdatedAt = domain.UpdatedAt ?? DateTime.UtcNow;
    }
}

