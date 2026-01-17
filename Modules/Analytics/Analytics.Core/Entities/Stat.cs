namespace Analytics.Core.Entities;

public class Stat
{
    public Guid Id { get; set; }
    public string MetricCode { get; set; } = default!;
    public Metric Metric { get; set; } = default!;
    public Scope Scope { get; set; }
    public Guid? ScopeIdentifier { get; set; }
    public string Value { get; set; } = default!;
    public string? AcademicYear { get; set; } = default!;
    public DateTime RecordedAt { get; set; } = DateTime.UtcNow;

}
