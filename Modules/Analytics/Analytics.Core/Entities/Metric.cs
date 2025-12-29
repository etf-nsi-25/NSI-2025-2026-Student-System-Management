namespace Analytics.Core.Entities;

public class Metric
{
	public int Id { get; set; }

	public string Name { get; set; } = string.Empty;

	public string Value { get; set; } = string.Empty;

	public Guid FacultyId { get; set; }
	public string UserId { get; set; } = string.Empty;

	public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}
