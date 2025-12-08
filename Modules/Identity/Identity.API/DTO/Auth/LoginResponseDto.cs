namespace Identity.API.DTO.Auth;

public class LoginResponseDto
{
    public string AccessToken { get; set; } = string.Empty;
<<<<<<< Updated upstream
=======
    public string RefreshToken { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
>>>>>>> Stashed changes
    public string TokenType { get; set; } = "Bearer";
}

