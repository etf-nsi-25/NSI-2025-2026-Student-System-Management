using Moq;
using Identity.Application.Services;
using Identity.Core.Interfaces.Repositories;
using Identity.Core.Interfaces.Services;
using Microsoft.Extensions.Logging;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Core.Models;
using Identity.Core.DTO;
using FluentAssertions;
using Xunit;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Identity.Tests.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IIdentityService> _mockIdentityService;
        private readonly Mock<IJwtTokenService> _mockTokenService;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockIdentityService = new Mock<IIdentityService>();
            _mockTokenService = new Mock<IJwtTokenService>();
            _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _mockTokenService.Object,
                _mockIdentityService.Object,
                _mockRefreshTokenRepo.Object,
                _mockLogger.Object
            );
        }

        private UserResponse CreateValidUserResponse(string id, string email, UserStatus status = UserStatus.Active)
        {
            return new UserResponse
            {
                Id = id,
                Username = email,
                Email = email,
                FirstName = "Test",
                LastName = "User",
                FacultyId = Guid.NewGuid(),
                Role = UserRole.Student,
                Status = status,
                IndexNumber = "12345"
            };
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            // Arrange
            var email = "test@test.com";
            var password = "Password123!";
            var userId = "user-abc-123";

            var userResponse = CreateValidUserResponse(userId, email);

            var accessToken = "access_token_example";
            var refreshTokenString = "refresh_token_example";
            var refreshToken = new RefreshToken 
            { 
                Token = refreshTokenString,
                UserId = userId,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _mockIdentityService.Setup(x => x.FindByEmailAsync(email))
                .ReturnsAsync(userResponse);

            _mockIdentityService.Setup(x => x.CheckPasswordAsync(userId, password))
                .ReturnsAsync(true);

            _mockTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<TokenClaims>()))
                .Returns(accessToken);

            _mockTokenService.Setup(x => x.CreateRefreshToken(userId))
                .Returns(refreshToken);

            // Act
            var result = await _authService.AuthenticateAsync(email, password);

            // Assert
            result.Should().NotBeNull();
            result.AccessToken.Should().Be(accessToken);
            result.RefreshToken.Should().Be(refreshTokenString);
            
            _mockRefreshTokenRepo.Verify(x => x.AddAsync(It.IsAny<RefreshToken>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorized_WhenUserNotFound()
        {
            // Arrange
            _mockIdentityService.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync((UserResponse?)null);

            // Act
            var act = async () => await _authService.AuthenticateAsync("test@test.com", "password");
            
            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password");
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorized_WhenPasswordIncorrect()
        {
            // Arrange
            var user = CreateValidUserResponse("id", "t@t.com");
            
            _mockIdentityService.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);
            
            _mockIdentityService.Setup(x => x.CheckPasswordAsync(user.Id, It.IsAny<string>()))
                .ReturnsAsync(false);

            // Act
            var act = async () => await _authService.AuthenticateAsync("t@t.com", "wrongpass");
                
            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password");
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorized_WhenUserIsDeactivated()
        {
            // Arrange
            var user = CreateValidUserResponse("id", "t@t.com", UserStatus.Inactive);

            _mockIdentityService.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            _mockIdentityService.Setup(x => x.CheckPasswordAsync(user.Id, It.IsAny<string>()))
                .ReturnsAsync(true);

            // Act
            var act = async () => await _authService.AuthenticateAsync("t@t.com", "pass");
            
            // Assert
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("User is deactivated");
        }
    }
}