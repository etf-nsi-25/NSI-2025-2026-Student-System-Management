using System.Collections.Generic;

namespace Identity.Application.DTO
{
    public enum TwoFAVerificationError
    {
        None = 0,
        InvalidCode,
        RateLimited,
        NotInitialized,
        UserNotFound,
        NotEnabled
    }

    public record TwoFAVerificationResult(
        bool Success,
        string? Message,
        IReadOnlyCollection<string>? RecoveryCodes = null,
        TwoFAVerificationError Error = TwoFAVerificationError.None);
}
