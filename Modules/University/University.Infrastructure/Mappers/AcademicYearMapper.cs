using University.Core.Entities;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Mappers
{
    public static class AcademicYearMapper
    {
        public static AcademicYear ToDomain(AcademicYearSchema schema)
        {
            if (schema == null) return null;

            return new AcademicYear
            {
                Id = schema.Id,
                Year = schema.Year,
                StartDate = schema.StartDate,
                EndDate = schema.EndDate,
                IsActive = schema.IsActive,
                CreatedAt = schema.CreatedAt,
                UpdatedAt = schema.UpdatedAt
            };
        }
    }
}
