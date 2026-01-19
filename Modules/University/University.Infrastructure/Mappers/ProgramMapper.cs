using University.Core.Entities;
using University.Infrastructure.Entities;

namespace University.Infrastructure.Mappers
{
    public static class ProgramMapper
    {
        public static Program ToDomain(ProgramSchema schema, bool includeRelationships = true)
        {
            if (schema == null) return null;

            var program = new Program
            {
                Id = schema.Id,
                DepartmentId = schema.DepartmentId,
                Name = schema.Name,
                Code = schema.Code,
                DegreeType = schema.DegreeType,
                DurationYears = schema.DurationYears,
                Credits = schema.Credits,
                CreatedAt = schema.CreatedAt,
                UpdatedAt = schema.UpdatedAt
            };

            if (includeRelationships && schema.Department != null)
            {
                program.Department = DepartmentMapper.ToDomain(schema.Department, false);
            }

            return program;
        }
    }
}
