using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Faculty.Core.Entities
{
	public class Student
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public string IndexNumber { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public DateOnly EnrollmentDate { get; set; }


	}
}
