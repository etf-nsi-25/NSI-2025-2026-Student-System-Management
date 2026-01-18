namespace University.Core.Entities
{
    public class Program
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string DegreeType { get; set; } = default!;
        public int DurationYears { get; set; }
        public int Credits { get; set; }

        public Department Department { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
