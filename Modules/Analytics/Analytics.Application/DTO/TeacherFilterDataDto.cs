using System.Collections.Generic;

namespace Analytics.Application.DTO;

public class TeacherFilterDataDto
{
    public List<string> Courses { get; set; } = new List<string>();
    public List<string> Years { get; set; } = new List<string>();
}