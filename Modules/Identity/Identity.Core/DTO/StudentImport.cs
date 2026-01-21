using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Identity.Core.DTO
{
    public class StudentImport
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? IndexNumber { get; set; }
        public string Role { get; set; }
        public DateTime EnrollmentDate { get; set; }
        public Guid FacultyId { get; set; }
        public List<string>? Errors { get; set; }
    }
}
