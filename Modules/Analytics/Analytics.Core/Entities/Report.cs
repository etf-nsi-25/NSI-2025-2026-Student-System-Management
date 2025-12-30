namespace Analytics.Core.Entities;

public class Report
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

	public string? Url { get; set; }
}
