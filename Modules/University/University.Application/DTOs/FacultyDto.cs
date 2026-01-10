namespace University.Application.DTOs
{
    public class FacultyDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Address { get; set; }
        public string Code { get; set; } = string.Empty;
    }
}
