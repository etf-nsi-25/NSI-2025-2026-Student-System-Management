using System.ComponentModel.DataAnnotations;

namespace Support.Core.Entities
{
    public class IssueCategory
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Title { get; set; } = string.Empty;
        public int Priority { get; set; }
    }
}
