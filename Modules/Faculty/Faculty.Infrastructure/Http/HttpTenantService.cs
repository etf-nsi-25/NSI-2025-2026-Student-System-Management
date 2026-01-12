using Common.Core.Tenant;
using Microsoft.AspNetCore.Http;

namespace Faculty.Infrastructure.Http;

/// <summary>
/// HTTP-based implementation of ITenantService that extracts TenantId from authenticated user claims.
/// </summary>
public class HttpTenantService(IHttpContextAccessor httpContextAccessor, IScopedTenantContext scopedTenantContext)
    : ITenantService
{
    private const string TenantIdClaimType = "tenantId";

    /// <summary>
    /// Gets the current Tenant ID. First an attempt will be made to read it from the ambient tenant context.
    /// If this is not available we will attempt to read it from the authenticated user's claims.
    /// </summary>
    /// <returns>The Tenant ID (Guid) if available.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when TenantId claim is missing or user is not authenticated.</exception>
    public Guid GetCurrentFacultyId()
    {
        var tenantScopeValue = scopedTenantContext.CurrentTenantId();
        if (tenantScopeValue != null)
        {
            return tenantScopeValue.Value;
        }

        // No scoped value found, attempt to pull tenancy info from http context.
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new UnauthorizedAccessException("HttpContext is not available. Ensure the service is used within an HTTP request context.");
        }

        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var tenantIdClaim = user.FindFirst(TenantIdClaimType);
        if (tenantIdClaim == null)
        {
            throw new UnauthorizedAccessException($"TenantId claim not found in user claims.");
        }

        if (!Guid.TryParse(tenantIdClaim.Value, out var tenantId))
        {
            throw new UnauthorizedAccessException($"Invalid TenantId format in claim: {tenantIdClaim.Value}. Expected a Guid.");
        }

        return tenantId;
    }
}