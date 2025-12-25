using System.Security.Claims;
using Faculty.Infrastructure.Http;
using Microsoft.AspNetCore.Http;
using Moq;
using Xunit;

namespace Faculty.Tests
{
    /// <summary>
    /// Unit tests for HttpTenantService to verify correct TenantId resolution from claims.
    /// </summary>
    public class HttpTenantServiceTests
    {
        private readonly Mock<IHttpContextAccessor> _mockHttpContextAccessor;
        private readonly HttpTenantService _tenantService;
        private readonly ITenantContext _tenantContext = new MockTenantContext();

        public HttpTenantServiceTests()
        {
            _mockHttpContextAccessor = new Mock<IHttpContextAccessor>();
            _tenantService = new HttpTenantService(_mockHttpContextAccessor.Object, _tenantContext);
        }

        #region Successful Resolution Tests

        [Fact]
        public void GetCurrentFacultyId_ShouldReturnTenantId_WhenValidGuidClaimExists()
        {
            // Arrange
            Guid expectedTenantId = Guid.NewGuid();
            var claims = new List<Claim>
            {
                new Claim("tenantId", expectedTenantId.ToString()),
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
        public void GetCurrentFacultyId_ShouldReturnTenantId_WhenTenantContextIsPopulated()
        {
            var expectedTenantId = Guid.NewGuid();
            _tenantContext.CurrentFacultyId = expectedTenantId;
            
            Assert.Equal(expectedTenantId, _tenantService.GetCurrentFacultyId());
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

        #region Invalid Guid Tests

        [Fact]
        public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenTenantIdClaimIsMissing()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, "testuser")
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
            Assert.Contains("TenantId claim not found in user claims.", exception.Message);
        }

        [Fact]
        public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenTenantIdClaimIsEmpty()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("tenantId", "")
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
        }

        [Fact]
        public void GetCurrentFacultyId_ShouldThrowUnauthorizedAccessException_WhenTenantIdClaimIsInvalidGuid()
        {
            // Arrange
            var claims = new List<Claim>
            {
                new Claim("tenantId", "not-a-guid")
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
            Assert.Contains("not-a-guid", exception.Message);
        }

        #endregion

        private class MockTenantContext : ITenantContext
        {
            public Guid? CurrentFacultyId { get; set; }

            public IDisposable Use(Guid _)
            {
                throw new NotImplementedException();
            }
        }
    }
}
