using Faculty.Core.Enums;

namespace Faculty.Core.Entities
{
    public class Faculty
    {
        public Guid Id { get; set; }
        
        public string Name { get; set; } = string.Empty; // Max length 200
        
        public string? Abbreviation { get; set; } // Max length 20
        
        public string? Description { get; set; } // Max length 1000
        
        public FacultyStatus Status { get; set; } = FacultyStatus.Active;
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        
        // Navigation properties for future relationship checks (not needed for this ticket)
        // public ICollection<User> Users { get; set; } = new List<User>();
        // public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}