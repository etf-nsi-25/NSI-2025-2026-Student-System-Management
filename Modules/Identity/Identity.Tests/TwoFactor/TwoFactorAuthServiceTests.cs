using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Identity.Application.Services;
using Identity.Core.DomainServices;
using Identity.Core.Interfaces.Services; 
using Identity.Core.Enums;
using Identity.Core.DTO; 

namespace Identity.Tests.TwoFactor
{
    public class TwoFactorAuthServiceTests
    {
        private readonly Mock<IIdentityService> _identityServiceMock; 
        private readonly Mock<ITotpProvider> _totpProviderMock;
        private readonly TwoFactorDomainService _domainService;
        private readonly TwoFactorAuthService _service;

        public TwoFactorAuthServiceTests()
        {
            _identityServiceMock = new Mock<IIdentityService>();
            _totpProviderMock = new Mock<ITotpProvider>();

            _domainService = new TwoFactorDomainService(_totpProviderMock.Object);

            _service = new TwoFactorAuthService(
                _identityServiceMock.Object,
                _domainService
            );
        }

        private UserResponse CreateValidUserResponse(string id, string username)
        {
            return new UserResponse
            {
                Id = id,
                Username = username,
                Email = username + "@test.com",
                FirstName = "First",
                LastName = "Last",
                FacultyId = Guid.NewGuid(),
                Role = UserRole.Student,
                Status = UserStatus.Active
            };
        }

        [Fact]
        public async Task EnableTwoFactorAsync_ReturnsSetupResult_WhenUserExists()
        {
            // Arrange
            var userId = "user-123";
            var username = "testuser";
            var user = CreateValidUserResponse(userId, username);

            _identityServiceMock
                .Setup(s => s.FindByIdAsync(userId))
                .ReturnsAsync(user);

            _totpProviderMock
                .Setup(p => p.GenerateSecret())
                .Returns("SECRET-XYZ");

            _totpProviderMock
                .Setup(p => p.GenerateQrCode(username, "SECRET-XYZ"))
                .Returns("QR-BASE64");

            // Act
            var result = await _service.EnableTwoFactorAsync(userId);

            // Assert
            result.ManualKey.Should().Be("SECRET-XYZ");
            result.QrCodeImageBase64.Should().Be("QR-BASE64");
        }

        [Fact]
        public async Task EnableTwoFactorAsync_Throws_WhenUserNotFound()
        {
            // Arrange
            var userId = "unknown-id";

            _identityServiceMock
                .Setup(s => s.FindByIdAsync(userId))
                .ReturnsAsync((UserResponse?)null); 

            // Act
            Func<Task> act = async () =>
                await _service.EnableTwoFactorAsync(userId);

            // Assert
            await act.Should().ThrowAsync<InvalidOperationException>();
        }

        [Fact]
        public async Task VerifySetupAsync_ReturnsSuccess_WhenCodeValid()
        {
            // Arrange
            _totpProviderMock
                .Setup(p => p.ValidateCode(It.IsAny<string>(), "123456"))
                .Returns(true);

            // Act
            var result = await _service.VerifySetupAsync("1", "123456");

            // Assert
            result.Success.Should().BeTrue();
            result.Message.Should().BeNull();
        }

        [Fact]
        public async Task VerifySetupAsync_ReturnsFailure_WhenCodeInvalid()
        {
            // Arrange
            _totpProviderMock
                .Setup(p => p.ValidateCode(It.IsAny<string>(), "999999"))
                .Returns(false);

            // Act
            var result = await _service.VerifySetupAsync("1", "999999");

            // Assert
            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid or expired code. Please try again.");
        }

        [Fact]
        public async Task VerifyLoginAsync_ReturnsSuccess_WhenCodeValid()
        {
            _totpProviderMock
                .Setup(p => p.ValidateCode(It.IsAny<string>(), "111111"))
                .Returns(true);

            var result = await _service.VerifyLoginAsync("1", "111111");

            result.Success.Should().BeTrue();
            result.Message.Should().BeNull();
        }

        [Fact]
        public async Task VerifyLoginAsync_ReturnsFailure_WhenCodeInvalid()
        {
            _totpProviderMock
                .Setup(p => p.ValidateCode(It.IsAny<string>(), "000000"))
                .Returns(false);

            var result = await _service.VerifyLoginAsync("1", "000000");

            result.Success.Should().BeFalse();
            result.Message.Should().Be("Invalid or expired code. Please try again.");
        }
    }
}