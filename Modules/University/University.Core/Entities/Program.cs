namespace University.Core.Entities
{
    public class Program
    {
        public Guid Id { get; set; }
        public Guid DepartmentId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string DegreeType { get; set; } = default!;
        public int DurationYears { get; set; }
        public int Credits { get; set; }
    }
}