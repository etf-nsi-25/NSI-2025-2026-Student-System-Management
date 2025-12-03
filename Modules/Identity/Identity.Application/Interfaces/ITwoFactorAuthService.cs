using System.Threading.Tasks;
using Identity.Application.DTO;

namespace Identity.Application.Interfaces
{
    public interface ITwoFactorAuthService
    {
        Task<TwoFASetupResult> EnableTwoFactorAsync(string userId);
        Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code);
        Task<TwoFAVerificationResult> VerifyLoginAsync(string userId, string code);
    }
}
