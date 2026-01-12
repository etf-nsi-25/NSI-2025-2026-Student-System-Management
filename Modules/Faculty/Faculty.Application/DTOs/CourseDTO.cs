using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Application.DTOs
{
    public class CourseDTO
    {
        public Guid Id { get; set; }
        [Required]
        [MinLength(2)]
        public string Name { get; set; }

        [Required]
        public string Code { get; set; }

        [Required]
        [RegularExpression("^(mandatory|elective)$",
            ErrorMessage = "Type must be either 'Mandatory' or 'Elective'.")]
        public string Type { get; set; }

        [Required]
        public string ProgramId { get; set; }

        [Range(1, 15, ErrorMessage = "ECTS must be between 1 and 15.")]
        public int ECTS { get; set; }
    }
}
