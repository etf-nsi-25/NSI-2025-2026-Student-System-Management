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
    public class TwoFactorFlowTests
    {
        private class FakeTotpProvider : ITotpProvider
        {
            public string LastGeneratedSecret { get; private set; } = string.Empty;

            public string GenerateSecret()
            {
                LastGeneratedSecret = "INTEGRATION-SECRET";
                return LastGeneratedSecret;
            }

            public string GenerateQrCode(string username, string secret)
            {
                return $"QR:{username}:{secret}";
            }

            public bool ValidateCode(string secret, string code)
            {
                return code == "123456";
            }
        }

        private UserResponse CreateFlowUser(string userId)
        {
            return new UserResponse
            {
                Id = userId,
                Username = "flow-user",
                Email = "flow@test.com",
                FirstName = "F",
                LastName = "L",
                FacultyId = Guid.NewGuid(),
                Role = UserRole.Student,
                Status = UserStatus.Active
            };
        }

        [Fact]
        public async Task Full2FASetupAndLoginFlow_Succeeds_WithCorrectCodes()
        {
            // Arrange
            var userId = "flow-user-id";
            var userResponse = CreateFlowUser(userId);

            var identityServiceMock = new Mock<IIdentityService>();
            identityServiceMock
                .Setup(s => s.FindByIdAsync(userId))
                .ReturnsAsync(userResponse);

            var totpProvider = new FakeTotpProvider();
            var domainService = new TwoFactorDomainService(totpProvider);
            var service = new TwoFactorAuthService(identityServiceMock.Object, domainService);

            var setupResult = await service.EnableTwoFactorAsync(userId);

            setupResult.ManualKey.Should().NotBeNullOrEmpty();
            setupResult.QrCodeImageBase64.Should().NotBeNullOrEmpty();

            var setupVerify = await service.VerifySetupAsync(userId, "123456");

            setupVerify.Success.Should().BeTrue();
            setupVerify.Message.Should().BeNull();

            var loginResult = await service.VerifyLoginAsync(userId, "123456");

            loginResult.Success.Should().BeTrue();
            loginResult.Message.Should().BeNull();
        }

        [Fact]
        public async Task SetupOrLogin_Fails_WithIncorrectCodes()
        {
            // Arrange
            var userId = "flow-user-id";
            var userResponse = CreateFlowUser(userId);

            var identityServiceMock = new Mock<IIdentityService>();
            identityServiceMock
                .Setup(s => s.FindByIdAsync(userId))
                .ReturnsAsync(userResponse);

            var totpProvider = new FakeTotpProvider();
            var domainService = new TwoFactorDomainService(totpProvider);
            var service = new TwoFactorAuthService(identityServiceMock.Object, domainService);

            // === Setup confirmation with incorrect code ===
            var badSetup = await service.VerifySetupAsync(userId, "000000");

            badSetup.Success.Should().BeFalse();
            badSetup.Message.Should().Be("Invalid or expired code. Please try again.");

            // === Login attempt with incorrect code ===
            var badLogin = await service.VerifyLoginAsync(userId, "888888");

            badLogin.Success.Should().BeFalse();
            badLogin.Message.Should().Be("Invalid or expired code. Please try again.");
        }
    }
}