using Moq;
using Identity.Application.Services;
using Identity.Core.Interfaces.Repositories;
using Identity.Core.Interfaces.Services;
using Identity.Core.Repositories;
using Identity.Core.Services;
using Microsoft.Extensions.Logging;
using Identity.Core.Entities;

using Identity.Core.Enums;
using Identity.Core.Models;
using FluentAssertions;

namespace Identity.Tests.Auth
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<IIdentityHasherService> _mockHasher;
        private readonly Mock<IJwtTokenService> _mockTokenService;
        private readonly Mock<IRefreshTokenRepository> _mockRefreshTokenRepo;
        private readonly Mock<ILogger<AuthService>> _mockLogger;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _mockUserRepo = new Mock<IUserRepository>();
            _mockHasher = new Mock<IIdentityHasherService>();
            _mockTokenService = new Mock<IJwtTokenService>();
            _mockRefreshTokenRepo = new Mock<IRefreshTokenRepository>();
            _mockLogger = new Mock<ILogger<AuthService>>();

            _authService = new AuthService(
                _mockTokenService.Object,
                _mockHasher.Object,
                _mockRefreshTokenRepo.Object,
                _mockUserRepo.Object,
                _mockLogger.Object
            );
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldReturnTokens_WhenCredentialsAreValid()
        {
            // Arrange
            var email = "test@test.com";
            var password = "password";
            var user = User.Create("username", "hash", "First", "Last", email, Guid.NewGuid(), UserRole.Student);
            var accessToken = "access_token";
            var refreshTokenString = "refresh_token_string";
            var refreshToken = new RefreshToken 
            { 
                Token = refreshTokenString,
                UserId = user.Id,
                ExpiresAt = DateTime.UtcNow.AddDays(7)
            };

            _mockUserRepo.Setup(x => x.GetByEmailAsync(email, It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);

            _mockHasher.Setup(x => x.VerifyPassword(user, password, It.IsAny<string>()))
                .Returns(true);

            _mockTokenService.Setup(x => x.GenerateAccessToken(It.IsAny<TokenClaims>()))
                .Returns(accessToken);

            _mockTokenService.Setup(x => x.CreateRefreshToken(user.Id))
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
            _mockUserRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((User?)null);

            // Act & Assert
            var act = async () => await _authService.AuthenticateAsync("test@test.com", "password");
            
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password");
        }

        [Fact]
        public async Task AuthenticateAsync_ShouldThrowUnauthorized_WhenPasswordIncorrect()
        {
            // Arrange
            var user = User.Create("u", "hash", "f", "l", "test@test.com", Guid.NewGuid(), UserRole.Student);
            
            _mockUserRepo.Setup(x => x.GetByEmailAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(user);
            
            _mockHasher.Setup(x => x.VerifyPassword(user, "wrongpass", It.IsAny<string>()))
                .Returns(false);

            // Act & Assert
            var act = async () => await _authService.AuthenticateAsync("test@test.com", "wrongpass");
                
            await act.Should().ThrowAsync<UnauthorizedAccessException>()
                .WithMessage("Invalid email or password");
        }
    }
}
