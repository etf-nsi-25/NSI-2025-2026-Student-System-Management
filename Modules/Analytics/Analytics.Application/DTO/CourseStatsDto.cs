namespace Analytics.Application.DTO;

public class CourseStatsDto
{
  public Guid CourseId { get; set; }
  public Dictionary<string, int> Distribution { get; set; } = new Dictionary<string, int>();
  public int TotalCount { get; set; }
  public int PassedCount { get; set; }
}
