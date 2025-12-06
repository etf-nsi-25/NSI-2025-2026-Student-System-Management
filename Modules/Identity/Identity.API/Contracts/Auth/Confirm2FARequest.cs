namespace Identity.API.Contracts.Auth;

public class Confirm2FARequest
{
    public string Code { get; set; } = default!;
}
