namespace Support.Application.DTOs
{
    public class CreateRequestDto
    {
        public int StudentId { get; set; }
        public string RequestType { get; set; } = string.Empty;
        public Guid FacultyId { get; set; }          
    }
}
