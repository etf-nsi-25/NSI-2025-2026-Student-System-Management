using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Support.Application.DTOs
{
    public class CreateDocumentRequestDTO
    {
        [Required(ErrorMessage = "UserId is required.")]
        public string UserId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "FacultyId must be greater than 0.")]
        public int FacultyId { get; set; }

        [Required(ErrorMessage = "DocumentType is required.")]
        [StringLength(100, ErrorMessage = "DocumentType cannot exceed 100 characters.")]
        public string DocumentType { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [RegularExpression("Pending|Approved|Rejected",
            ErrorMessage = "Status must be Pending, Approved, or Rejected.")]
        public string Status { get; set; }
    }
}