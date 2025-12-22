using System.Security.Claims;
using Faculty.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Faculty.Core.Services;

/// <summary>
/// HTTP-based implementation of ITenantService that extracts TenantId from authenticated user claims.
/// </summary>
public class HttpTenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string TenantIdClaimType = "tenantId";

    public HttpTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Gets the current Tenant ID from the authenticated user's claims.
    /// </summary>
    /// <returns>The Tenant ID (Guid) if available.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when TenantId claim is missing or user is not authenticated.</exception>
    public Guid GetCurrentFacultyId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new UnauthorizedAccessException("HttpContext is not available. Ensure the service is used within an HTTP request context.");
        }

        var path = httpContext.Request.Path.Value;

        // Ako je endpoint statistike univerziteta, vrati dummy ID
        if (path != null && path.StartsWith("/api/statistics", StringComparison.OrdinalIgnoreCase))
        {
            return Guid.Empty; // univerzitetska statistika – ne filtrira po fakultetu
        }

        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var tenantIdClaim = user.FindFirst(TenantIdClaimType);
        if (tenantIdClaim == null)
        {
            throw new UnauthorizedAccessException($"TenantId claim not found in user claims. Available claims: {string.Join(", ", user.Claims.Select(c => c.Type))}");
        }

        if (!Guid.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            throw new UnauthorizedAccessException($"Invalid TenantId format in claim: {tenantIdClaim.Value}. Expected a Guid.");
        }

        return tenantId;
    }
}

