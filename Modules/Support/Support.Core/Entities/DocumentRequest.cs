using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Core.Entities
{
	public class DocumentRequest
	{
		public int Id { get; set; }
		public string UserId { get; set; }
		public Guid FacultyId { get; set; }
		public string DocumentType { get; set; }
		public string Status { get; set; }
		public DateTime CreatedAt { get; set; }
		public DateTime? CompletedAt { get; set; }
	}

}
