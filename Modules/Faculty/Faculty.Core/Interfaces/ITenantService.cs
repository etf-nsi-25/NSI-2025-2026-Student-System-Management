namespace Faculty.Core.Interfaces;

/// <summary>
/// Service for resolving the current tenant (Faculty) ID from the authenticated user context.
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// Gets the current Faculty ID from the authenticated user's claims.
    /// </summary>
    /// <returns>The Faculty ID (int) if available.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when FacultyId claim is missing or user is not authenticated.</exception>
    int GetCurrentFacultyId();
}

