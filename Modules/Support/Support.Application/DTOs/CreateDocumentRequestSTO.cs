using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Application.DTOs
{
	public class CreateDocumentRequestDTO
	{
		public string UserId { get; set; }
		public int FacultyId { get; set; }
		public string DocumentType { get; set; }
		public string Status { get; set; }
	}
}
