using University.Core.Entities;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Mappers
{
    public static class DepartmentMapper
    {
        public static Department ToDomain(DepartmentSchema schema, bool includeRelationships = true)
        {
            if (schema == null) return null;

            var department = new Department
            {
                Id = schema.Id,
                FacultyId = schema.FacultyId,
                Name = schema.Name,
                Code = schema.Code,
                HeadOfDepartmentId = schema.HeadOfDepartmentId,
                CreatedAt = schema.CreatedAt,
                UpdatedAt = schema.UpdatedAt
            };

            if (includeRelationships)
            {
                if (schema.Faculty != null)
                {
                    department.Faculty = FacultyMapper.ToDomain(schema.Faculty, false);
                }

                if (schema.Programs != null)
                {
                    department.Programs = schema.Programs
                        .Select(p => ProgramMapper.ToDomain(p, false))
                        .ToList();
                }
            }

            return department;
        }
    }
}
