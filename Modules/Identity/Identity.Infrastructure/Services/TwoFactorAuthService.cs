using System;
using System.Threading.Tasks;
using Identity.Application.Interfaces;
using Identity.Core.DomainServices;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;
using Identity.Application.DTO;

namespace Identity.Infrastructure.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ITotpProvider _totpProvider;

        public TwoFactorAuthService(
            UserManager<ApplicationUser> userManager,
            ISecretEncryptionService encryption)
        {
            _userManager = userManager;
            _totpProvider = totpProvider;
        }

        // STEP 1: Generate secret + QR + save encrypted secret
        public async Task<TwoFASetupResult> EnableTwoFactorAsync(string userId)
        {
            Console.WriteLine(">>> USER ID RECEIVED = " + userId);
            var guid = Guid.Parse(userId);

            // DOMAIN user
            var domainUser = await _userRepository.GetByIdAsync(guid);
            if (domainUser == null)
                throw new InvalidOperationException("Domain user not found.");

            // IDENTITY user
            var identityUser = await _userManager.FindByIdAsync(userId);
            if (identityUser == null)
                throw new InvalidOperationException("Identity user not found.");

            // Generate raw secret + QR
            var (secret, qrCode) = _twoFactorDomain.GenerateSetupFor(domainUser.Username);

            // Encrypt secret
            var encrypted = _encryption.Encrypt(secret);

            // Save encrypted secret into AspNetUsers
            identityUser.TwoFactorSecretEncrypted = encrypted;
            await _userManager.UpdateAsync(identityUser);

            return new TwoFASetupResult(secret, qrCode);
        }

        public Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code)
        {
            const string demoSecret = "IGNORED";
            bool ok = _twoFactorDomain.VerifyCode(demoSecret, code);

            if (!ok)
                return Task.FromResult(new TwoFAVerificationResult(false, "Invalid or expired code"));

            return Task.FromResult(new TwoFAVerificationResult(true, null));
        }

        public Task<TwoFAVerificationResult> VerifyLoginAsync(string userId, string code)
        {
            const string demoSecret = "IGNORED";
            bool ok = _twoFactorDomain.VerifyCode(demoSecret, code);

            if (!ok)
                return Task.FromResult(new TwoFAVerificationResult(false, "Invalid login code"));

            return Task.FromResult(new TwoFAVerificationResult(true, null));
        }
    }
}