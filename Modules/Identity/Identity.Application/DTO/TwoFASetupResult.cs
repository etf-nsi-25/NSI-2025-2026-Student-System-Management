namespace Identity.Application.DTO
{
    public record TwoFASetupResult(
        string ManualKey,
        string QrCodeImageBase64
    );
}
