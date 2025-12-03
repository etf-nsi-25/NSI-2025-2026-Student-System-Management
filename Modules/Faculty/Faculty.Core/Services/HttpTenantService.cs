using System.Security.Claims;
using Faculty.Core.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Faculty.Core.Services;

/// <summary>
/// HTTP-based implementation of ITenantService that extracts FacultyId from authenticated user claims.
/// </summary>
public class HttpTenantService : ITenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private const string FacultyIdClaimType = "FacultyId";

    public HttpTenantService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    /// <summary>
    /// Gets the current Faculty ID from the authenticated user's claims.
    /// </summary>
    /// <returns>The Faculty ID (int) if available.</returns>
    /// <exception cref="UnauthorizedAccessException">Thrown when FacultyId claim is missing or user is not authenticated.</exception>
    public int GetCurrentFacultyId()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext == null)
        {
            throw new UnauthorizedAccessException("HttpContext is not available. Ensure the service is used within an HTTP request context.");
        }

        var user = httpContext.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            throw new UnauthorizedAccessException("User is not authenticated.");
        }

        var facultyIdClaim = user.FindFirst(FacultyIdClaimType);
        if (facultyIdClaim == null)
        {
            throw new UnauthorizedAccessException($"FacultyId claim not found in user claims. Available claims: {string.Join(", ", user.Claims.Select(c => c.Type))}");
        }

        if (!int.TryParse(facultyIdClaim.Value, out var facultyId))
        {
            throw new UnauthorizedAccessException($"Invalid FacultyId format in claim: {facultyIdClaim.Value}. Expected an integer.");
        }

        return facultyId;
    }
}

