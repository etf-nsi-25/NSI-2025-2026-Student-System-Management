using System;
using System.Threading.Tasks;
using FluentAssertions;
using Identity.Application.Services;
using Identity.Core.DTO;
using Identity.Core.Interfaces.Services;
using Moq;
using Xunit;

namespace Identity.Tests.TwoFactor;

public class TwoFactorAuthServiceTests
{
    private readonly Mock<IIdentityService> _identityServiceMock;
    private readonly TwoFactorAuthService _service;

    public TwoFactorAuthServiceTests()
    {
        _identityServiceMock = new Mock<IIdentityService>();
        _service = new TwoFactorAuthService(_identityServiceMock.Object);
    }

    [Fact]
    public async Task EnableTwoFactorAsync_ReturnsSetupResult_WhenIdentityServiceReturnsSetup()
    {
        // Arrange
        var userId = "user-123";
        _identityServiceMock
            .Setup(s => s.GenerateTwoFactorSetupAsync(userId, It.IsAny<string>()))
            .ReturnsAsync(new TwoFactorSetupInfo(
                ManualKey: "SECRET-XYZ",
                QrCodeImageBase64: "data:image/png;base64,AAA",
                OtpAuthUri: "otpauth://totp/x"));

        // Act
        var result = await _service.EnableTwoFactorAsync(userId);

        // Assert
        result.ManualKey.Should().Be("SECRET-XYZ");
        result.QrCodeImageBase64.Should().StartWith("data:image/png;base64,");
    }

    [Fact]
    public async Task EnableTwoFactorAsync_Throws_WhenUserNotFound()
    {
        // Arrange
        var userId = "unknown-id";
        _identityServiceMock
            .Setup(s => s.GenerateTwoFactorSetupAsync(userId, It.IsAny<string>()))
            .ThrowsAsync(new InvalidOperationException("User not found"));

        // Act
        Func<Task> act = async () => await _service.EnableTwoFactorAsync(userId);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task VerifySetupAsync_ReturnsSuccess_WhenCodeValid()
    {
        // Arrange
        _identityServiceMock
            .Setup(s => s.ConfirmTwoFactorSetupAsync("1", "123456"))
            .ReturnsAsync(true);

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
        _identityServiceMock
            .Setup(s => s.ConfirmTwoFactorSetupAsync("1", "999999"))
            .ReturnsAsync(false);

        // Act
        var result = await _service.VerifySetupAsync("1", "999999");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid or expired code. Please try again.");
    }

    [Fact]
    public async Task VerifyLoginAsync_ReturnsSuccess_WhenCodeValid()
    {
        // Arrange
        _identityServiceMock
            .Setup(s => s.VerifyTwoFactorCodeAsync("1", "111111"))
            .ReturnsAsync(true);

        // Act
        var result = await _service.VerifyLoginAsync("1", "111111");

        // Assert
        result.Success.Should().BeTrue();
        result.Message.Should().BeNull();
    }

    [Fact]
    public async Task VerifyLoginAsync_ReturnsFailure_WhenCodeInvalid()
    {
        // Arrange
        _identityServiceMock
            .Setup(s => s.VerifyTwoFactorCodeAsync("1", "000000"))
            .ReturnsAsync(false);

        // Act
        var result = await _service.VerifyLoginAsync("1", "000000");

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid or expired code. Please try again.");
    }
}