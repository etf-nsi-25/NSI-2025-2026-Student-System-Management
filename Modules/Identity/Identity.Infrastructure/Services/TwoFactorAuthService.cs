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

        public async Task<TwoFASetupResult> EnableTwoFactorAsync(string userId)
        {
            // Validate GUID
            if (!Guid.TryParse(userId, out var guid))
                return new TwoFASetupResult(false, "Invalid user id format.");

            // DOMAIN USER
            var domainUser = await _userRepository.GetByIdAsync(guid);
            if (domainUser == null)
                return new TwoFASetupResult(false, "Domain user does not exist.");

            // IDENTITY USER
            var identityUser = await _userManager.FindByIdAsync(userId);
            if (identityUser == null)
                return new TwoFASetupResult(false, "Identity user does not exist.");

            // Already enabled?
            if (identityUser.TwoFactorEnabled)
                return new TwoFASetupResult(false, "Two-factor authentication is already enabled.");

            // Already pending?
            if (!string.IsNullOrEmpty(identityUser.TwoFactorSecretPending))
                return new TwoFASetupResult(
                    false,
                    "A 2FA setup is already in progress. Verify it first."
                );

            // Username fallback if domainUser.Username is null
            var username = !string.IsNullOrWhiteSpace(domainUser.Username)
                ? domainUser.Username
                : identityUser.Email ?? identityUser.UserName;

            // Generate secret+QR
            var (secret, qrCode) = _twoFactorDomain.GenerateSetupFor(username);

            // Encrypt
            var encrypted = _encryption.Encrypt(secret);

            // Save pending secret
            identityUser.TwoFactorSecretPending = encrypted;
            identityUser.TwoFactorEnabled = false;

            var res = await _userManager.UpdateAsync(identityUser);
            if (!res.Succeeded)
                return new TwoFASetupResult(false, "Failed to update user record.");

            return new TwoFASetupResult(true, "Successfully enabled initial data", secret, qrCode);
        }

        public async Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new(false, "User does not exist.");

            if (string.IsNullOrEmpty(user.TwoFactorSecretPending))
                return new(false, "2FA setup was not initialized.");

            var secret = _encryption.Decrypt(user.TwoFactorSecretPending);

            bool ok = _twoFactorDomain.VerifyCode(secret, code);
            if (!ok)
                return new(false, "Invalid or expired verification code.");

            // CONFIRM PERMANENTLY
            user.TwoFactorSecretEncrypted = user.TwoFactorSecretPending;
            user.TwoFactorSecretPending = null;
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
