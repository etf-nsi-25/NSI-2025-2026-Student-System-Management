namespace Faculty.Application.DTOs;

public class SubjectProgressDTO
{
    public string ContextLabel { get; set; } = string.Empty;
    public List<SubjectProgressItemDTO> Items { get; set; } = new();
}

public class SubjectProgressItemDTO
{
    public string Code { get; set; } = string.Empty;
    public double Percent { get; set; }
    public string Status { get; set; } = string.Empty;
}

