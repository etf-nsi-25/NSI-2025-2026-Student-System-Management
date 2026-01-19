namespace Faculty.Application.DTOs;

public class StudentAssignmentOverviewDTO
{
    public int AssignmentId { get; set; } 
    
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public DateTime? DueDate { get; set; }

    public int? MaxPoints { get; set; } 
    
    public string Status { get; set; } = string.Empty;
    public DateTime? SubmissionDate { get; set; }
    public int? Grade { get; set; }
    public int? Points { get; set; }
    public string? Feedback { get; set; }
}