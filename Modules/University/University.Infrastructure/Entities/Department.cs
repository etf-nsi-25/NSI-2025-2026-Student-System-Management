namespace University.Infrastructure.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public Guid FacultyId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? HeadOfDepartment { get; set; }

        public Faculty Faculty { get; set; } = default!;
        public ICollection<Program> Programs { get; set; } = new List<Program>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
