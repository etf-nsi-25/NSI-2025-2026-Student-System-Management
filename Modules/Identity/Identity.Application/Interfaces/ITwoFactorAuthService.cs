using System.Threading.Tasks;

namespace Identity.Application.Interfaces
{
    public interface ITwoFactorAuthService
    {
        Task<TwoFASetupResult> EnableTwoFactorAsync(string userId);
        Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code);
        Task<TwoFAVerificationResult> VerifyLoginAsync(string userId, string code);
    }
    
    public record TwoFASetupResult(string ManualKey, string QrCodeImageBase64);

    public record TwoFAVerificationResult(bool Success, string? Message);
}
