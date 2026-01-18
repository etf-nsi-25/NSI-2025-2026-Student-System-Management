namespace Faculty.Application.DTOs
{
    public class CreateAssignmentDTO
    {
        public Guid CourseId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime? DueDate { get; set; }
        public int? MaxPoints { get; set; }
    }
}
