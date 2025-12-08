namespace University.Core.Entities
{
    public class Department
    {
        public Guid Id { get; set; }
        public Guid FacultyId { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? HeadOfDepartment { get; set; }
    }
}