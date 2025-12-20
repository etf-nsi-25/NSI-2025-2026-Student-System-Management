using System.Reflection;
using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Xunit;
using Identity.Application.Services;
using Identity.Core.DomainServices;
using Identity.Core.Entities;
using Identity.Core.Repositories;

namespace Identity.Tests.TwoFactor
{
    public class TwoFactorAuthServiceTests
    {
        private readonly Mock<IUserRepository> _userRepositoryMock;
        private readonly Mock<ITotpProvider> _totpProviderMock;
        private readonly TwoFactorDomainService _domainService;
        private readonly TwoFactorAuthService _service;

        public TwoFactorAuthServiceTests()
        {
            _userRepositoryMock = new Mock<IUserRepository>();
            _totpProviderMock = new Mock<ITotpProvider>();

            _domainService = new TwoFactorDomainService(_totpProviderMock.Object);

            _service = new TwoFactorAuthService(
                _userRepositoryMock.Object,
                _domainService
            );
        }

        [Fact]
        public async Task EnableTwoFactorAsync_ReturnsSetupResult_WhenUserExists()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;

            var usernameProp = typeof(User).GetProperty(
                "Username",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            usernameProp!.SetValue(user, "testuser");

            var idProp = typeof(User).GetProperty(
                "Id",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            idProp!.SetValue(user, userId);

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            _totpProviderMock
                .Setup(p => p.GenerateSecret())
                .Returns("SECRET-XYZ");

            _totpProviderMock
                .Setup(p => p.GenerateQrCode("testuser", "SECRET-XYZ"))
                .Returns("QR-BASE64");

            // Act
            var result = await _service.EnableTwoFactorAsync(userId.ToString());

            // Assert
            result.ManualKey.Should().Be("SECRET-XYZ");
            result.QrCodeImageBase64.Should().Be("QR-BASE64");
        }

        [Fact]
        public async Task EnableTwoFactorAsync_Throws_WhenUserNotFound()
        {
            // Arrange
            var userId = Guid.NewGuid();

            _userRepositoryMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync((User?)null);

            // Act
            Func<Task> act = async () =>
                await _service.EnableTwoFactorAsync(userId.ToString());

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
