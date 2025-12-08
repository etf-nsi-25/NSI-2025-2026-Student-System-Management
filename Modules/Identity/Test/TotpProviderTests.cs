using System;
using Identity.Core.Configuration;
using Identity.Infrastructure.TOTP;
using Microsoft.Extensions.Options;
using OtpNet;
using Xunit;

public class TotpProviderTests
{
    private TotpProvider CreateProvider()
    {
        var settings = Options.Create(new TotpSettings
        {
            Issuer = "StudentSystem",
            StepSeconds = 30,
            Digits = 6,
            SecretLength = 20,
            QrPixelsPerModule = 10
        });

        return new TotpProvider(settings);
    }

    [Fact]
    public void GenerateSecret_Returns_NonEmpty_Base32_String()
    {
        var provider = CreateProvider();

        var secret = provider.GenerateSecret();

        Assert.False(string.IsNullOrWhiteSpace(secret));
    }

    [Fact]
    public void ValidateCode_ReturnsTrue_ForValidCode()
    {
        var provider = CreateProvider();
        var secret = provider.GenerateSecret();

        // Decode Base32 secret to bytes
        var secretBytes = Base32Encoding.ToBytes(secret);
        var totp = new Totp(secretBytes, step: 30, totpSize: 6);

        var code = totp.ComputeTotp(DateTime.UtcNow);

        var isValid = provider.ValidateCode(secret, code);

        Assert.True(isValid);
    }

    [Fact]
    public void ValidateCode_ReturnsFalse_ForInvalidCode()
    {
        var provider = CreateProvider();
        var secret = provider.GenerateSecret();

        var isValid = provider.ValidateCode(secret, "000000");

        Assert.False(isValid);
    }
}