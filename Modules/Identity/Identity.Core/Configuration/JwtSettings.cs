namespace Identity.Core.Configuration;

public class JwtSettings
{
    public string Issuer { get; set; }
    public string Audience { get; set; }
    public int AccessTokenExpirationMinutes { get; set; } = 30;
    public int RefreshTokenExpirationDays { get; set; } = 7;
    public string SigningKey { get; set; }
}