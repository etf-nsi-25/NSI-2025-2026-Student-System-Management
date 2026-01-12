namespace Identity.API.DTO.Auth;

public class PublicKeyResponseDto
{
    public string PublicKey { get; set; } = string.Empty;
    public string Algorithm { get; set; } = "RS256";
    public string KeyId { get; set; } = string.Empty;
}

