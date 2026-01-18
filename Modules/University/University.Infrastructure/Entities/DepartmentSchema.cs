using Identity.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using University.Core.Entities;

namespace University.Infrastructure.Entities
{
    public class DepartmentSchema
    {
        public int Id { get; set; }
        public int FacultyId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public Guid HeadOfDepartmentId { get; set; }

        public FacultySchema Faculty { get; set; } = default!;
        public ICollection<ProgramSchema> Programs { get; set; } = new List<ProgramSchema>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
