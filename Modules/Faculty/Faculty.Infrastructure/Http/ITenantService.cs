namespace Faculty.Infrastructure.Http;

/// <summary>
/// Service for resolving the current tenant ID from the authenticated user context.
/// </summary>
public interface ITenantService
{
    /// <summary>
    /// Gets the current Tenant ID from the authenticated user's claims.
    /// </summary>
    /// <returns>The Tenant ID (int) if available.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when TenantId claim is missing or user is not authenticated.</exception>
    Guid GetCurrentFacultyId();
}

