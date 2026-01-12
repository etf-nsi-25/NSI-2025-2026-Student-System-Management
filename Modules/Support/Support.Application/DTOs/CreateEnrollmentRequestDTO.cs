using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Application.DTOs
{
	public class CreateEnrollmentRequestDTO
	{
		public string UserId { get; set; }
		public Guid FacultyId { get; set; }
		public string AcademicYear { get; set; } = default!;
		public int Semester { get; set; } 
	}
}
