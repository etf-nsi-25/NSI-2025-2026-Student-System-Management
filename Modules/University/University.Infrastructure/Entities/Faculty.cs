using Identity.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace University.Infrastructure.Entities
{
    public class Faculty
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; }
        public string Code { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime EstablishedDate { get; set; }
        public Guid DeanId { get; set; }

        public ICollection<Department> Departments { get; set; } = new List<Department>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
