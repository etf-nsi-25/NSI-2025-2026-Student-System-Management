using System.ComponentModel.DataAnnotations;

namespace University.Core.Entities
{
    public class Faculty
    {
        public int Id { get; set; }
        public string Name { get; set; } = default!;
        public string Address { get; set; } = default!;
        public string Code { get; set; } = default!;
        public string? Description { get; set; }
        public DateTime EstablishedDate { get; set; }
        public Guid DeanId { get; set; }
        
        public ICollection<Department> Departments { get; set; } = new List<Department>();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
