using Faculty.Core.Enums;

namespace Faculty.Application.DTOs
{
    public class FacultyFilterDto
    {
        public string? Name { get; set; }
        public FacultyStatus? Status { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}