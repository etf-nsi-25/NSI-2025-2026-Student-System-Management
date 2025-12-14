namespace Faculty.Application.DTOs
{
    public class StudentRequestDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string StudentIndex { get; set; } = string.Empty;
        public string RequestType { get; set; } = string.Empty;
        public string RequestDetails { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}