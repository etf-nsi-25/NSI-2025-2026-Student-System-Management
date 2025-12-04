using System;
using System.Threading.Tasks;
using Identity.Application.DTO;
using Identity.Application.Interfaces;
using Identity.Core.DomainServices;
using Identity.Core.Repositories;
using Identity.Infrastructure.Entities;
using Microsoft.AspNetCore.Identity;

namespace Identity.Infrastructure.Services
{
    public class TwoFactorAuthService : ITwoFactorAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly TwoFactorDomainService _twoFactorDomain;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ISecretEncryptionService _encryption;

        public TwoFactorAuthService(
            IUserRepository userRepository,
            TwoFactorDomainService twoFactorDomain,
            UserManager<ApplicationUser> userManager,
            ISecretEncryptionService encryption
        )
        {
            _userRepository = userRepository;
            _twoFactorDomain = twoFactorDomain;
            _userManager = userManager;
            _encryption = encryption;
        }

        // STEP 1: Generate secret + QR + save encrypted secret
        public async Task<TwoFASetupResult> EnableTwoFactorAsync(string userId)
        {
            var guid = Guid.Parse(userId);

            // DOMAIN USER
            var domainUser = await _userRepository.GetByIdAsync(guid);
            if (domainUser == null)
                throw new InvalidOperationException("Domain user not found.");

            // IDENTITY USER
            var identityUser = await _userManager.FindByIdAsync(userId);
            if (identityUser == null)
                throw new InvalidOperationException("Identity user not found.");

            // Generate raw secret + QR (this uses Eldarov TOTP provider)
            var (secret, qrCode) = _twoFactorDomain.GenerateSetupFor(domainUser.Username);

            // Encrypt raw secret
            var encrypted = _encryption.Encrypt(secret);

            // Save to AspNetUsers
            identityUser.TwoFactorSecretEncrypted = encrypted;
            identityUser.TwoFactorEnabled = false; // not confirmed yet
            await _userManager.UpdateAsync(identityUser);

            // Return raw secret + QR to frontend
            return new TwoFASetupResult(secret, qrCode);
        }
        public async Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new(false, "User does not exist.");

            if (string.IsNullOrEmpty(user.TwoFactorSecretEncrypted))
                return new(false, "Two-factor authentication has not been initialized for this user.");

            var secret = _encryption.Decrypt(user.TwoFactorSecretEncrypted);

            bool ok = _twoFactorDomain.VerifyCode(secret, code);

            if (!ok)
                return new(false, "The verification code is invalid or has expired. Please try again.");

            user.TwoFactorEnabled = true;
            await _userManager.UpdateAsync(user);

            return new(true, "Two-factor authentication has been successfully activated.");
        }

        public async Task<TwoFAVerificationResult> VerifyLoginAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new(false, "User does not exist.");

            if (!user.TwoFactorEnabled)
                return new(false, "Two-factor authentication is not enabled for this user.");

            var secret = _encryption.Decrypt(user.TwoFactorSecretEncrypted);

            bool ok = _twoFactorDomain.VerifyCode(secret, code);

            if (!ok)
                return new(false, "Invalid code. Please try again.");

            return new(true, "Login successful.");
        }
    }
}