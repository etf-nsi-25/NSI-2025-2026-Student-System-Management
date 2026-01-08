using Faculty.Core.Entities;
using Faculty.Infrastructure.Schemas;

namespace Faculty.Infrastructure.Mappers;

/// <summary>
/// Maps between Exam domain entities and persistence entities.
/// </summary>
public static class ExamMapper
{
    public static ExamSchema ToPersistence(Exam domain)
    {
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        return new ExamSchema
        {
            Id = domain.Id,
            FacultyId = domain.FacultyId,
            CourseId = domain.CourseId,
            Name = domain.Name,
            ExamDate = domain.ExamDate,
            RegDeadline = domain.RegDeadline,
            CreatedAt = domain.CreatedAt,
            UpdatedAt = domain.UpdatedAt
        };
    }

    public static Exam ToDomain(ExamSchema persistence, bool includeRelationships = false)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));

        var domain = new Exam
        {
            Id = persistence.Id,
            FacultyId = persistence.FacultyId,
            CourseId = persistence.CourseId,
            Name = persistence.Name,
            ExamDate = persistence.ExamDate,
            RegDeadline = persistence.RegDeadline,
            CreatedAt = persistence.CreatedAt,
            UpdatedAt = persistence.UpdatedAt
        };

        if (includeRelationships)
        {
            if (persistence.Course != null)
            {
                domain.Course = CourseMapper.ToDomain(persistence.Course, false);
            }

            if (persistence.ExamRegistrations != null)
            {
                domain.ExamRegistrations = ExamRegistrationMapper.ToDomainCollection(persistence.ExamRegistrations, false).ToList();
            }
            else
            {
                domain.ExamRegistrations = new List<ExamRegistration>();
            }
        }
        else
        {
            domain.ExamRegistrations = new List<ExamRegistration>();
        }

        return domain;
    }

    public static IEnumerable<ExamSchema> ToPersistenceCollection(IEnumerable<Exam> domainCollection)
    {
        if (domainCollection == null)
            return Enumerable.Empty<ExamSchema>();

        return domainCollection.Select(ToPersistence);
    }

    public static IEnumerable<Exam> ToDomainCollection(IEnumerable<ExamSchema> persistenceCollection, bool includeRelationships = false)
    {
        if (persistenceCollection == null)
            return Enumerable.Empty<Exam>();

        return persistenceCollection.Select(p => ToDomain(p, includeRelationships));
    }

    public static void UpdatePersistence(ExamSchema persistence, Exam domain)
    {
        if (persistence == null)
            throw new ArgumentNullException(nameof(persistence));
        if (domain == null)
            throw new ArgumentNullException(nameof(domain));

        persistence.CourseId = domain.CourseId;
        persistence.Name = domain.Name;
        persistence.ExamDate = domain.ExamDate;
        persistence.RegDeadline = domain.RegDeadline;
        persistence.UpdatedAt = domain.UpdatedAt ?? DateTime.UtcNow;
    }
}

