namespace Identity.API.DTO.Auth;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = "Bearer";

    public bool RequiresTwoFactor { get; set; }

    public string? TwoFactorToken { get; set; }
}

