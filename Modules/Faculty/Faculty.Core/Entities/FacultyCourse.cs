using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Core.Entities
{
    public class FacultyCourse
    {
        public Guid Id { get; set; } 
        public Guid CourseIdFromUniversity { get; set; } 

        public Guid FacultyId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int Ects { get; set; }
        public string AcademicYear { get; set; }
        public string Semester { get; set; }
        public Guid ProfessorId { get; set; }
    }
}
