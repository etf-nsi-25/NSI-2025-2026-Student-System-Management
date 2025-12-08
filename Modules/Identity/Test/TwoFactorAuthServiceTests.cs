using System;
using System.Threading.Tasks;
using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Identity.Core.DomainServices;
using Identity.Core.Entities;
using Identity.Core.Enums;
using Identity.Core.Repositories;
using Identity.Infrastructure.Entities;
using Identity.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Moq;
using Xunit;

public class TwoFactorAuthServiceTests
{
    private class FakeTotpProvider : ITotpProvider
    {
        public string SecretToReturn { get; set; } = "SECRETBASE32";

        public string GenerateSecret() => SecretToReturn;

        public TotpSetupArtifacts GenerateSetupArtifacts(string username, string secret)
            => new TotpSetupArtifacts(
                $"otpauth://totp/StudentSystem:{username}?secret={secret}",
                "data:image/png;base64,FAKEQR");

        public bool ValidateCode(string secret, string code)
            => secret == SecretToReturn && code == "123456";
    }

    private class FakeEncryptionService : ISecretEncryptionService
    {
        public string Encrypt(string plaintext) => "enc:" + plaintext;
        public string Decrypt(string ciphertext) =>
            ciphertext.StartsWith("enc:") ? ciphertext.Substring(4) : ciphertext;
    }

    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly FakeTotpProvider _totpFake;
    private readonly TwoFactorDomainService _domainService;
    private readonly ISecretEncryptionService _encryption;
    private readonly TwoFactorAuthService _service;

    public TwoFactorAuthServiceTests()
    {
        _userRepoMock = new Mock<IUserRepository>();

        var userStoreMock = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            userStoreMock.Object,
            null, null, null, null, null, null, null, null);

        _totpFake = new FakeTotpProvider();
        _domainService = new TwoFactorDomainService(_totpFake);
        _encryption = new FakeEncryptionService();

        _service = new TwoFactorAuthService(
            _userRepoMock.Object,
            _domainService,
            _userManagerMock.Object,
            _encryption);
    }

    private static User CreateDomainUser(Guid id)
    {
        var user = User.Create(
            "student",
            "hash",
            "Ime",
            "Prezime",
            "student@example.com",
            Guid.NewGuid(),
            UserRole.Student,
            "IB12345");
        user.SetId(id);
        return user;
    }

    private static ApplicationUser CreateIdentityUser(string id)
    {
        return new ApplicationUser
        {
            Id = id,
            Email = "student@example.com",
            TwoFactorEnabled = false,
            TwoFactorSecretEncrypted = null,
            TwoFactorSecretPending = null
        };
    }

    [Fact]
    public async Task EnableTwoFactorAsync_SuccessfullyStartsSetup()
    {
        var userId = Guid.NewGuid().ToString();
        var guid = Guid.Parse(userId);

        var domainUser = CreateDomainUser(guid);
        var identityUser = CreateIdentityUser(userId);

        _userRepoMock.Setup(r => r.GetByIdAsync(guid))
                     .ReturnsAsync(domainUser);

        _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                        .ReturnsAsync(identityUser);

        _userManagerMock.Setup(m => m.UpdateAsync(identityUser))
                        .ReturnsAsync(IdentityResult.Success);

        var result = await _service.EnableTwoFactorAsync(userId);

        Assert.True(result.Success);
        Assert.Equal("Two-factor authentication setup initiated.", result.Message);
        Assert.Equal(_totpFake.SecretToReturn, result.ManualEntryKey);
        Assert.Equal("data:image/png;base64,FAKEQR", result.QrCodeBase64);
        Assert.Contains("otpauth://totp/StudentSystem", result.OtpAuthUri);
        Assert.Equal("enc:" + _totpFake.SecretToReturn, identityUser.TwoFactorSecretPending);
        Assert.False(identityUser.TwoFactorEnabled);
    }

    [Fact]
    public async Task VerifySetupAsync_WithCorrectCode_Enables2FA_AndGeneratesRecoveryCodes()
    {
        var userId = Guid.NewGuid().ToString();
        var identityUser = new ApplicationUser
        {
            Id = userId,
            Email = "student@example.com",
            TwoFactorEnabled = false,
            TwoFactorSecretPending = "enc:" + _totpFake.SecretToReturn,
            TwoFactorSecretEncrypted = null
        };

        _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                        .ReturnsAsync(identityUser);

        _userManagerMock.Setup(m => m.UpdateAsync(identityUser))
                        .ReturnsAsync(IdentityResult.Success);

        var result = await _service.VerifySetupAsync(userId, "123456");

        Assert.True(result.Success);
        Assert.Equal("Two-factor authentication has been successfully activated.", result.Message);
        Assert.NotNull(result.RecoveryCodes);
        Assert.NotEmpty(result.RecoveryCodes);
        Assert.True(identityUser.TwoFactorEnabled);
        Assert.Null(identityUser.TwoFactorSecretPending);
        Assert.NotNull(identityUser.TwoFactorSecretEncrypted);
        Assert.False(string.IsNullOrWhiteSpace(identityUser.RecoveryCodesHashed));
    }

    [Fact]
    public async Task VerifySetupAsync_WithInvalidCode_ReturnsInvalidCodeError()
    {
        var userId = Guid.NewGuid().ToString();
        var identityUser = new ApplicationUser
        {
            Id = userId,
            Email = "student@example.com",
            TwoFactorEnabled = false,
            TwoFactorSecretPending = "enc:" + _totpFake.SecretToReturn
        };

        _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                        .ReturnsAsync(identityUser);


        var result = await _service.VerifySetupAsync(userId, "000000");

        Assert.False(result.Success);
        Assert.Equal(TwoFAVerificationError.InvalidCode, result.Error);
    }

    [Fact]
    public async Task VerifyLoginAsync_WithEnabled2FA_AndCorrectCode_Succeeds()
    {
        var userId = Guid.NewGuid().ToString();
        var identityUser = new ApplicationUser
        {
            Id = userId,
            Email = "student@example.com",
            TwoFactorEnabled = true,
            TwoFactorSecretEncrypted = "enc:" + _totpFake.SecretToReturn
        };

        _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                        .ReturnsAsync(identityUser);

        var result = await _service.VerifyLoginAsync(userId, "123456");

        Assert.True(result.Success);
        Assert.Equal("Login successful.", result.Message);
        Assert.Equal(TwoFAVerificationError.None, result.Error);
    }

    [Fact]
    public async Task VerifyLoginAsync_WithWrongCode_ReturnsInvalidCodeError()
    {
        var userId = Guid.NewGuid().ToString();
        var identityUser = new ApplicationUser
        {
            Id = userId,
            Email = "student@example.com",
            TwoFactorEnabled = true,
            TwoFactorSecretEncrypted = "enc:" + _totpFake.SecretToReturn
        };

        _userManagerMock.Setup(m => m.FindByIdAsync(userId))
                        .ReturnsAsync(identityUser);

        var result = await _service.VerifyLoginAsync(userId, "000000");

        Assert.False(result.Success);
        Assert.Equal(TwoFAVerificationError.InvalidCode, result.Error);
    }
}