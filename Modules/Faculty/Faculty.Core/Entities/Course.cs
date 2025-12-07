using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Core.Entities
{
    public enum CourseType
    {
        Mandatory = 1,
        Elective = 2
    }
    public class Course
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public CourseType Type { get; set; }
        public string ProgramId { get; set; }
        public int ECTS { get; set; }
    }
}
