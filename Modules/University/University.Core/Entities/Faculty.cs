using System.ComponentModel.DataAnnotations;

namespace University.Core.Entities
{
    public class Faculty
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        [Required]
        public string Code { get; set; } = string.Empty;
    }
}
