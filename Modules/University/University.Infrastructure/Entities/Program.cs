using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Infrastructure.Entities
{
    public class Program
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string DegreeType { get; set; } = default!;
        public int DurationYears { get; set; }
        public int Credits { get; set; }

        public Department Department { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
