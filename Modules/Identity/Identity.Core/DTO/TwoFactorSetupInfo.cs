namespace Identity.Core.DTO;

public record TwoFactorSetupInfo(string ManualKey, string QrCodeImageBase64, string OtpAuthUri);
