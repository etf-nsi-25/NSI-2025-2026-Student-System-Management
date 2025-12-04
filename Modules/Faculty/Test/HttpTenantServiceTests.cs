using System.Security.Claims;
using Faculty.Core.Services;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Faculty.Test;

/// <summary>
/// Unit tests for HttpTenantService to verify correct TenantId resolution from claims.
/// </summary>
public class HttpTenantServiceTests
{
    private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
    private readonly HttpTenantService _tenantService;

    public HttpTenantServiceTests()
    {
        _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
        _tenantService = new HttpTenantService(_mockHttpContextAccessor.Object);
    }

    #region Successful Resolution Tests

    [Fact]
    public void GetCurrentFacultyId_ShouldReturnTenantId_WhenValidClaimExists()
    {
        // Arrange
        const int expectedTenantId = 42;
        var claims = new List<Claim>
        {
            new Claim("tenant_id", expectedTenantId.ToString()),
            new Claim(ClaimTypes.Name, "testuser")
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _tenantService.GetCurrentFacultyId();

        // Assert
        Assert.Equal(expectedTenantId, result);
    }

    [Fact]
    public void GetCurrentFacultyId_ShouldReturnLargeValue_WhenTenantIdIsLarge()
    {
        // Arrange
        const int expectedTenantId = int.MaxValue;
        var claims = new List<Claim>
        {
            new Claim("tenant_id", expectedTenantId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _tenantService.GetCurrentFacultyId();

        // Assert
        Assert.Equal(expectedTenantId, result);
    }

    [Fact]
    public void GetCurrentFacultyId_ShouldReturnMinimumValue_WhenTenantIdIsMinInt()
    {
        // Arrange
        const int expectedTenantId = int.MinValue;
        var claims = new List<Claim>
        {
            new Claim("tenant_id", expectedTenantId.ToString())
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act
        var result = _tenantService.GetCurrentFacultyId();

        // Assert
        Assert.Equal(expectedTenantId, result);
    }

    #endregion

    #region HttpContext Null Tests

    [Fact]
    public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenHttpContextIsNull()
    {
        // Arrange
        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns((HttpContext)null!);

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _tenantService.GetCurrentFacultyId());
        Assert.Contains("HttpContext is not available", exception.Message);
    }

    #endregion

    #region Authentication Tests

    [Fact]
    public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenUserIsNull()
    {
        // Arrange
        var httpContext = new DefaultHttpContext
        {
            User = null!
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _tenantService.GetCurrentFacultyId());
        Assert.Contains("User is not authenticated", exception.Message);
    }

    [Fact]
    public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenIdentityIsNull()
    {
        // Arrange
        var principal = new ClaimsPrincipal();
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _tenantService.GetCurrentFacultyId());
        Assert.Contains("User is not authenticated", exception.Message);
    }

    [Fact]
    public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenNotAuthenticated()
    {
        // Arrange
        var identity = new ClaimsIdentity(); // Not authenticated
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _tenantService.GetCurrentFacultyId());
        Assert.Contains("User is not authenticated", exception.Message);
    }

    #endregion

    #region Invalid Format Tests

    [Fact]
    public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenTenantIdIsEmptyString()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("tenant_id", string.Empty)
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _tenantService.GetCurrentFacultyId());
        Assert.Contains("Invalid TenantId format", exception.Message);
        Assert.Contains("Expected an integer", exception.Message);
    }

    [Fact]
    public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenTenantIdIsDecimal()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("tenant_id", "42.5")
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _tenantService.GetCurrentFacultyId());
        Assert.Contains("Invalid TenantId format", exception.Message);
        Assert.Contains("42.5", exception.Message);
        Assert.Contains("Expected an integer", exception.Message);
    }

    [Fact]
    public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenTenantIdIsOutOfRange()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("tenant_id", "99999999999999999999") // Too large for int
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _tenantService.GetCurrentFacultyId());
        Assert.Contains("Invalid TenantId format", exception.Message);
        Assert.Contains("Expected an integer", exception.Message);
    }

    [Fact]
    public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenTenantIdIsAlphaNumeric()
    {
        // Arrange
        var claims = new List<Claim>
        {
            new Claim("tenant_id", "abc123")
        };

        var identity = new ClaimsIdentity(claims, "Bearer");
        var principal = new ClaimsPrincipal(identity);
        var httpContext = new DefaultHttpContext
        {
            User = principal
        };

        _mockHttpContextAccessor.Setup(x => x.HttpContext).Returns(httpContext);

        // Act & Assert
        var exception = Assert.Throws<UnauthorizedAccessException>(() => _tenantService.GetCurrentFacultyId());
        Assert.Contains("Invalid TenantId format", exception.Message);
        Assert.Contains("abc123", exception.Message);
        Assert.Contains("Expected an integer", exception.Message);
    }

    #endregion

    #region Constructor Tests

    [Fact]
    public void Constructor_ShouldThrowArgumentNullException_WhenHttpContextAccessorIsNull()
    {
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new HttpTenantService(null!));
    }

    #endregion
}

