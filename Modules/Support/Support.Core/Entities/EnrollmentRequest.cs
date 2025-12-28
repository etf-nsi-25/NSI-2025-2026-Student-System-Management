using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Core.Entities
{
	public class EnrollmentRequest
	{
		public Guid Id { get; set; }

		public string UserId { get; set; }          
		public Guid FacultyId { get; set; }

		public string AcademicYear { get; set; } = default!;  
		public int Semester { get; set; }                     

		public string Status { get; set; } = "Pending";       

		public DateTime CreatedAt { get; set; }
		public DateTime? DecisionAt { get; set; }
		public string? DecidedByUserId { get; set; }            
		public string? DecisionNote { get; set; }             
	}
}
