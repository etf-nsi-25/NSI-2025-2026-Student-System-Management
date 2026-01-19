using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Application.DTOs;

public class StudentEnrollmentItemDto
{
    public int EnrollmentId { get; set; }
    public Guid CourseId { get; set; }
    public string CourseName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int? Grade { get; set; }
}
