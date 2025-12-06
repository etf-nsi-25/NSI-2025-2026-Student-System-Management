namespace Identity.Core.DomainServices
{
    public interface ITotpProvider
    {
        string GenerateSecret();
        TotpSetupArtifacts GenerateSetupArtifacts(string username, string secret);
        bool ValidateCode(string secret, string code);
    }

    public record TotpSetupArtifacts(string OtpAuthUri, string QrCodeBase64);
}
