namespace Identity.Core.Models;

public class PublicKeyInfo
{
    public string PublicKey { get; set; } = string.Empty;
    public string Algorithm { get; set; } = "RS256";
    public string KeyId { get; set; } = string.Empty;
}