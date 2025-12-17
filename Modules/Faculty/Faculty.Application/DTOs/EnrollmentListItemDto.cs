using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Application.DTOs
{
    public class EnrollmentListItemDto
    {
        public int EnrollmentId { get; set; }
        public Guid CourseId { get; set; }
        public string CourseName { get; set; } = "";
        public string? Status { get; set; }
        public int? Grade { get; set; }
    }
}
