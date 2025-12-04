using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Identity.Core.DomainServices;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Threading.Tasks;

namespace Identity.Infrastructure.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITotpProvider _totpProvider;

        public TwoFactorAuthService(
            UserManager<ApplicationUser> userManager,
            ITotpProvider totpProvider)
        {
            _userManager = userManager;
            _totpProvider = totpProvider;
        }

        public async Task<TwoFASetupResult> EnableTwoFactorAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                throw new Exception("User not found.");

            if (string.IsNullOrEmpty(user.TwoFactorSecretEncrypted))
            {
                user.TwoFactorSecretEncrypted = _totpProvider.GenerateSecret();
                await _userManager.UpdateAsync(user);
            }

            string username = user.Email ?? user.UserName ?? user.Id;

            string qrCodeBase64 = _totpProvider.GenerateQrCode(
                username,
                user.TwoFactorSecretEncrypted
            );

            return new TwoFASetupResult(
                ManualKey: user.TwoFactorSecretEncrypted,
                QrCodeImageBase64: qrCodeBase64
            );
        }


        public async Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new TwoFAVerificationResult(false, "User not found.");

            if (string.IsNullOrEmpty(user.TwoFactorSecretEncrypted))
                return new TwoFAVerificationResult(false, "2FA setup not generated yet.");

            bool valid = _totpProvider.ValidateCode(user.TwoFactorSecretEncrypted, code);

            if (!valid)
                return new TwoFAVerificationResult(false, "Invalid authentication code.");

            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);

            return new TwoFAVerificationResult(true, "Two-factor setup successful.");
        }

        public async Task<TwoFAVerificationResult> VerifyLoginAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new TwoFAVerificationResult(false, "User not found.");

            if (!user.TwoFactorEnabled)
                return new TwoFAVerificationResult(false, "Two-factor authentication not enabled.");

            if (string.IsNullOrEmpty(user.TwoFactorSecretEncrypted))
                return new TwoFAVerificationResult(false, "2FA secret key missing.");

            bool valid = _totpProvider.ValidateCode(user.TwoFactorSecretEncrypted, code);

            if (!valid)
                return new TwoFAVerificationResult(false, "Invalid authentication code.");

            return new TwoFAVerificationResult(true, "Verification successful.");
        }
    }
}
