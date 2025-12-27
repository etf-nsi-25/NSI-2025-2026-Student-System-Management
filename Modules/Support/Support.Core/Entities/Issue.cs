using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Support.Core.Entities
{
    public class Issue
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        [ForeignKey("IssueCategory")]
        public int CategoryId { get; set; }
        public IssueCategory? Category { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        public Status Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? ClosedAt { get; set; }
    }
}
