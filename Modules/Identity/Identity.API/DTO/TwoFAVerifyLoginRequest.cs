namespace Identity.API.DTO;

public class TwoFAVerifyLoginRequest
{
    public string Code { get; set; } = string.Empty;

    public string TwoFactorToken { get; set; } = string.Empty;
}
