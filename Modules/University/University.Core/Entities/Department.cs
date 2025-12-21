namespace University.Core.Entities
{
    public class Department
    {
        public int Id { get; set; }
        public Faculty Faculty { get; set; }
        public string Name { get; set; } = default!;
        public string Code { get; set; } = default!;
        public Guid HeadOfDepartment { get; set; }
    }
}
