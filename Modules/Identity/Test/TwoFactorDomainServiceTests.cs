using Identity.Core.DomainServices;
using Moq;
using Xunit;

public class TwoFactorDomainServiceTests
{
    private readonly Mock<ITotpProvider> _totpMock;
    private readonly TwoFactorDomainService _service;

    public TwoFactorDomainServiceTests()
    {
        _totpMock = new Mock<ITotpProvider>();
        _service = new TwoFactorDomainService(_totpMock.Object);
    }

    [Fact]
    public void GenerateSetupFor_UsesTotpProvider_AndReturnsArtifacts()
    {
        // Arrange
        var username = "student@example.com";
        var secret = "SECRETBASE32";
        var qr = "data:image/png;base64,FAKE";
        var uri = "otpauth://totp/StudentSystem:student@example.com?secret=SECRETBASE32";

        _totpMock.Setup(t => t.GenerateSecret()).Returns(secret);
        _totpMock.Setup(t => t.GenerateSetupArtifacts(username, secret))
                 .Returns(new TotpSetupArtifacts(uri, qr));
                 
        var result = _service.GenerateSetupFor(username);

        Assert.Equal(secret, result.ManualEntryKey);
        Assert.Equal(qr, result.QrCodeBase64);
        Assert.Equal(uri, result.OtpAuthUri);

        _totpMock.Verify(t => t.GenerateSecret(), Times.Once);
        _totpMock.Verify(t => t.GenerateSetupArtifacts(username, secret), Times.Once);
    }

    [Fact]
    public void VerifyCode_Delegates_To_TotpProvider()
    {
        var secret = "SECRETBASE32";
        var code = "123456";

        _totpMock.Setup(t => t.ValidateCode(secret, code)).Returns(true);

        var ok = _service.VerifyCode(secret, code);

        Assert.True(ok);
        _totpMock.Verify(t => t.ValidateCode(secret, code), Times.Once);
    }
}