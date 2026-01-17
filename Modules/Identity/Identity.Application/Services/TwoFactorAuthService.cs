using System.Threading.Tasks;
using Identity.Application.Interfaces;
using Identity.Core.Interfaces.Services;

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Identity.Infrastructure")]

namespace Identity.Application.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly IIdentityService _identityService;
        private const string Issuer = "StudentSystem";

        public TwoFactorAuthService(
            IIdentityService identityService)
        {
            _identityService = identityService;
        }

        public async Task<TwoFASetupResult> EnableTwoFactorAsync(string userId)
        {
            var setup = await _identityService.GenerateTwoFactorSetupAsync(userId, Issuer);

            return new TwoFASetupResult(
                ManualKey: setup.ManualKey,
                QrCodeImageBase64: setup.QrCodeImageBase64);
        }

        public async Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code)
        {
            var ok = await _identityService.ConfirmTwoFactorSetupAsync(userId, code);

            return ok
                ? new TwoFAVerificationResult(true, null)
                : new TwoFAVerificationResult(false, "Invalid or expired code. Please try again.");
        }

        public async Task<TwoFAVerificationResult> VerifyLoginAsync(string userId, string code)
        {
            var ok = await _identityService.VerifyTwoFactorCodeAsync(userId, code);

            return ok
                ? new TwoFAVerificationResult(true, null)
                : new TwoFAVerificationResult(false, "Invalid or expired code. Please try again.");
        }
    }
}
