namespace Faculty.Application.DTOs
{
    public class FacultyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Abbreviation { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; } = string.Empty; // "Active" or "Inactive"
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}