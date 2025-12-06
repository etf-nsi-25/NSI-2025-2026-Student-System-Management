namespace Identity.Core.DomainServices
{
    public interface ITotpProvider
    {
        string GenerateSecret();
        string GenerateQrCode(string username, string secret);
        bool ValidateCode(string secret, string code);
    }
}
