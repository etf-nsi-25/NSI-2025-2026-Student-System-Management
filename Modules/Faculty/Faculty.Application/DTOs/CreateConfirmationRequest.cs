using System.ComponentModel.DataAnnotations;

namespace Faculty.Application.DTOs
{
    public class CreateConfirmationRequest
    {
        [Required]
        public string StudentIndex { get; set; } = string.Empty;

        [Required]
        public string RequestType { get; set; } = string.Empty;

        [Required]
        public string StatusRequest { get; set; } = string.Empty;

        public bool ShouldPrint { get; set; }
    }
}