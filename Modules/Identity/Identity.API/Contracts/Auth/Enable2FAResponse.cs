namespace Identity.API.Contracts.Auth;

public class Enable2FAResponse
{
    public string SharedKey { get; set; } = default!;
    public string AuthenticatorUri { get; set; } = default!;
    public bool AlreadyEnabled { get; set; }
}
