using System;
using System.Reflection;
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
    /// <summary>
    /// Integration-style tests at the module level.
    /// These tests run the real TwoFactorDomainService + TwoFactorAuthService
    /// with mocked repository and a fake TOTP provider.
    ///
    /// Covered scenarios:
    /// - 2FA setup flow
    /// - TOTP validation (valid and invalid codes)
    /// - Login flow that requires 2FA
    ///
    /// Note: The real application would issue JWT/refresh tokens after
    /// successful 2FA login. Here we only validate 2FA logic.
    /// </summary>
    public class TwoFactorFlowTests
    {
        /// <summary>
        /// A deterministic fake TOTP provider used for integration tests.
        /// </summary>
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
                // The exact format does not matter for testing.
                return $"QR:{username}:{secret}";
            }

            public bool ValidateCode(string secret, string code)
            {
                // In this simplified demo, "123456" is the only correct code.
                return code == "123456";
            }
        }

        /// <summary>
        /// Helper for constructing a User instance with non-public setters.
        /// </summary>
        private static User CreateUser(Guid id, string username)
        {
            var user = (User)Activator.CreateInstance(typeof(User), nonPublic: true)!;

            var usernameProp = typeof(User).GetProperty(
                "Username",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            usernameProp!.SetValue(user, username);

            var idProp = typeof(User).GetProperty(
                "Id",
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

            idProp!.SetValue(user, id);

            return user;
        }

        [Fact]
        public async Task Full2FASetupAndLoginFlow_Succeeds_WithCorrectCodes()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateUser(userId, "flow-user");

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var totpProvider = new FakeTotpProvider();
            var domainService = new TwoFactorDomainService(totpProvider);
            var service = new TwoFactorAuthService(userRepoMock.Object, domainService);

            // === Step 1: Start 2FA setup ===
            var setupResult = await service.EnableTwoFactorAsync(userId.ToString());

            setupResult.ManualKey.Should().NotBeNullOrEmpty();
            setupResult.QrCodeImageBase64.Should().NotBeNullOrEmpty();

            // === Step 2: Confirm setup using a valid TOTP code ===
            var setupVerify = await service.VerifySetupAsync(userId.ToString(), "123456");

            setupVerify.Success.Should().BeTrue();
            setupVerify.Message.Should().BeNull();

            // === Step 3: Login with 2FA using a valid TOTP code ===
            var loginResult = await service.VerifyLoginAsync(userId.ToString(), "123456");

            loginResult.Success.Should().BeTrue();
            loginResult.Message.Should().BeNull();
        }

        [Fact]
        public async Task SetupOrLogin_Fails_WithIncorrectCodes()
        {
            // Arrange
            var userId = Guid.NewGuid();
            var user = CreateUser(userId, "flow-user");

            var userRepoMock = new Mock<IUserRepository>();
            userRepoMock
                .Setup(r => r.GetByIdAsync(userId))
                .ReturnsAsync(user);

            var totpProvider = new FakeTotpProvider();
            var domainService = new TwoFactorDomainService(totpProvider);
            var service = new TwoFactorAuthService(userRepoMock.Object, domainService);

            // === Setup confirmation with incorrect code ===
            var badSetup = await service.VerifySetupAsync(userId.ToString(), "000000");

            badSetup.Success.Should().BeFalse();
            badSetup.Message.Should().Be("Invalid or expired code. Please try again.");

            // === Login attempt with incorrect code ===
            var badLogin = await service.VerifyLoginAsync(userId.ToString(), "888888");

            badLogin.Success.Should().BeFalse();
            badLogin.Message.Should().Be("Invalid or expired code. Please try again.");
        }
    }
}