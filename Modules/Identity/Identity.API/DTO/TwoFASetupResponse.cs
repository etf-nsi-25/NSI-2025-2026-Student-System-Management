namespace Identity.API.DTO;

public class TwoFASetupResponse
{
    public string QrCodeImageBase64 { get; set; } = string.Empty;
    public string ManualKey { get; set; } = string.Empty;
}
