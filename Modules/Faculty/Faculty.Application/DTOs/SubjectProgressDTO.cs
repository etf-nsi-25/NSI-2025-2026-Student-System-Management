namespace Faculty.Application.DTOs;

/// <summary>
/// DTO representing subject progress for a student.
/// </summary>
public class SubjectProgressDTO
{
    public string ContextLabel { get; set; } = string.Empty; // e.g., "for current semester"
    public List<SubjectProgressItemDTO> Items { get; set; } = new();
}

/// <summary>
/// DTO representing progress for a single subject.
/// </summary>
public class SubjectProgressItemDTO
{
    public string Code { get; set; } = string.Empty;
    public double Percent { get; set; }
    public string Status { get; set; } = string.Empty; // e.g., "In Progress", "Passed", "Failed", "Enrolled"
}

