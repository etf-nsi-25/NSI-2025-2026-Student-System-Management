namespace Faculty.Core.Interfaces;

/// <summary>
/// Interface for entities that are tenant-aware (belong to a specific Faculty).
/// </summary>
public interface ITenantAware
{
    /// <summary>
    /// The ID of the Faculty (tenant) that owns this entity.
    /// </summary>
    Guid FacultyId { get; set; }
}

