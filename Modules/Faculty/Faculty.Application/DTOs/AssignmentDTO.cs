namespace Faculty.Application.DTOs
{
    public class AssignmentDTO
    {
        public Guid Id { get; set; }
        public string Course { get; set; } = default!;
        public string Name { get; set; } = default!;
        public int MaxPoints { get; set; }
        public string Major { get; set; } = default!;
        public string Description { get; set; } = default!;
        public DateTime DueDate { get; set; }
    }

}
