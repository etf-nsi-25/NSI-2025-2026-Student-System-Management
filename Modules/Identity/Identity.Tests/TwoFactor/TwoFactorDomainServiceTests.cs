using FluentAssertions;
using Moq;
using Xunit;
using Identity.Core.DomainServices;

namespace Identity.Tests.TwoFactor
{
    public class TwoFactorDomainServiceTests
    {
        private readonly Mock<ITotpProvider> _totpProviderMock;
        private readonly TwoFactorDomainService _service;

        public TwoFactorDomainServiceTests()
        {
            _totpProviderMock = new Mock<ITotpProvider>();
            _service = new TwoFactorDomainService(_totpProviderMock.Object);
        }

        [Fact]
        public void GenerateSetupFor_ReturnsSecretAndQrCode()
        {
            // Arrange
            var username = "testuser";

            _totpProviderMock
                .Setup(p => p.GenerateSecret())
                .Returns("SECRET-123");

            _totpProviderMock
                .Setup(p => p.GenerateQrCode(username, "SECRET-123"))
                .Returns("QR-CODE-BASE64");

            // Act
            var (secret, qr) = _service.GenerateSetupFor(username);

            // Assert
            secret.Should().Be("SECRET-123");
            qr.Should().Be("QR-CODE-BASE64");

            _totpProviderMock.Verify(p => p.GenerateSecret(), Times.Once);
            _totpProviderMock.Verify(p => p.GenerateQrCode(username, "SECRET-123"), Times.Once);
        }

        [Fact]
        public void VerifyCode_ReturnsTrue_WhenProviderValidates()
        {
            _totpProviderMock
                .Setup(p => p.ValidateCode("SECRET", "123456"))
                .Returns(true);

            var result = _service.VerifyCode("SECRET", "123456");

            result.Should().BeTrue();
        }

        [Fact]
        public void VerifyCode_ReturnsFalse_WhenProviderFails()
        {
            _totpProviderMock
                .Setup(p => p.ValidateCode("SECRET", "999999"))
                .Returns(false);

            var result = _service.VerifyCode("SECRET", "999999");

            result.Should().BeFalse();
        }
    }
}