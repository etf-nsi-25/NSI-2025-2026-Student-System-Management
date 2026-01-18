namespace University.Core.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public Guid FacultyId { get; set; }
        public Faculty Faculty { get; set; } = default!;
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public Guid HeadOfDepartmentId { get; set; }
        
        public ICollection<Program> Programs { get; set; } = new List<Program>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
