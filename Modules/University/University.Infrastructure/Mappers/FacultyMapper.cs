using University.Core.Entities;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Mappers
{
    public static class FacultyMapper
    {
        public static Faculty ToDomain(FacultySchema schema, bool includeRelationships = true)
        {
            if (schema == null) return null;

            var faculty = new Faculty
            {
                Id = schema.Id,
                Name = schema.Name,
                Address = schema.Address,
                Code = schema.Code,
                Description = schema.Description,
                EstablishedDate = schema.EstablishedDate,
                DeanId = schema.DeanId,
                CreatedAt = schema.CreatedAt,
                UpdatedAt = schema.UpdatedAt
            };

            if (includeRelationships && schema.Departments != null)
            {
                faculty.Departments = schema.Departments
                    .Select(d => DepartmentMapper.ToDomain(d, false)) // Prevent cycles
                    .ToList();
            }

            return faculty;
        }

        public static FacultySchema ToSchema(Faculty domain)
        {
            if (domain == null) return null;

            return new FacultySchema
            {
                Id = domain.Id,
                Name = domain.Name,
                Address = domain.Address,
                Code = domain.Code,
                Description = domain.Description,
                EstablishedDate = domain.EstablishedDate,
                DeanId = domain.DeanId,
                CreatedAt = domain.CreatedAt,
                UpdatedAt = domain.UpdatedAt
            };
        }
    }
}
