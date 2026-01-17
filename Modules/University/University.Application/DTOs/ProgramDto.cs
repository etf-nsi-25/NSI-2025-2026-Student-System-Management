namespace  University.Application.DTOs
{
    public class ProgramDto
    {
        public int Id { get; set; }
        public int DepartmentId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string DegreeType { get; set; } = default!;
        public int DurationYears { get; set; }
        public int Credits { get; set; }
    }
}