using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
        private static readonly ConcurrentDictionary<string, AttemptWindow> SetupAttempts = new();
        private static readonly ConcurrentDictionary<string, AttemptWindow> LoginAttempts = new();
        private const int MaxAttempts = 5;
        private static readonly TimeSpan AttemptWindowDuration = TimeSpan.FromMinutes(5);

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
                    "Two-factor authentication setup is already in progress. Complete verification to continue.");

            // Username fallback if domainUser.Username is null
            var username = !string.IsNullOrWhiteSpace(domainUser.Username)
                ? domainUser.Username
                : identityUser.UserName
                    ?? identityUser.Email
                    ?? $"user-{userId}";

            // Generate secret+QR
            var (manualEntryKey, qrCode, otpauthUri) = _twoFactorDomain.GenerateSetupFor(username);

            // Encrypt
            var encrypted = _encryption.Encrypt(manualEntryKey);

            // Save pending secret
            identityUser.TwoFactorSecretPending = encrypted;
            identityUser.TwoFactorEnabled = false;
            identityUser.RecoveryCodesHashed = null;

            var res = await _userManager.UpdateAsync(identityUser);
            if (!res.Succeeded)
                return new TwoFASetupResult(false, "Failed to update user record.");

            return new TwoFASetupResult(
                true,
                "Two-factor authentication setup initiated.",
                manualEntryKey,
                qrCode,
                otpauthUri);
        }

        public async Task<TwoFAVerificationResult> VerifySetupAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new(false, "User does not exist.", null, TwoFAVerificationError.UserNotFound);

            if (!IsCodeFormatValid(code, out var formatError))
                return new(false, formatError, null, TwoFAVerificationError.InvalidCode);

            if (IsRateLimited(SetupAttempts, userId, out var retryAfter))
                return new(false, BuildRateLimitMessage(retryAfter), null, TwoFAVerificationError.RateLimited);

            if (string.IsNullOrEmpty(user.TwoFactorSecretPending))
                return new(false, "Two-factor authentication setup was not initialized.", null, TwoFAVerificationError.NotInitialized);

            var secret = _encryption.Decrypt(user.TwoFactorSecretPending);

            bool ok = _twoFactorDomain.VerifyCode(secret, code);
            if (!ok)
            {
                RegisterFailure(SetupAttempts, userId);
                if (IsRateLimited(SetupAttempts, userId, out var retry))
                    return new(false, BuildRateLimitMessage(retry), null, TwoFAVerificationError.RateLimited);
                return new(false, "Invalid or expired verification code.", null, TwoFAVerificationError.InvalidCode);
            }

            // CONFIRM PERMANENTLY
            user.TwoFactorSecretEncrypted = user.TwoFactorSecretPending;
            user.TwoFactorSecretPending = null;
            user.TwoFactorEnabled = true;
            var recoveryCodes = GenerateRecoveryCodes();
            var hashedCodes = HashRecoveryCodes(recoveryCodes);
            user.RecoveryCodesHashed = JsonSerializer.Serialize(hashedCodes);

            await _userManager.UpdateAsync(user);
            ResetAttempts(SetupAttempts, userId);

            return new(
                true,
                "Two-factor authentication has been successfully activated.",
                recoveryCodes,
                TwoFAVerificationError.None);
        }

        public async Task<TwoFAVerificationResult> VerifyLoginAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return new(false, "User does not exist.", null, TwoFAVerificationError.UserNotFound);

            if (!user.TwoFactorEnabled)
                return new(false, "Two-factor authentication is not enabled for this user.", null, TwoFAVerificationError.NotEnabled);

            if (!IsCodeFormatValid(code, out var formatError))
                return new(false, formatError, null, TwoFAVerificationError.InvalidCode);

            if (IsRateLimited(LoginAttempts, userId, out var retryAfter))
                return new(false, BuildRateLimitMessage(retryAfter), null, TwoFAVerificationError.RateLimited);

            if (string.IsNullOrEmpty(user.TwoFactorSecretEncrypted))
                return new(false, "Two-factor secret is missing. Reconfigure 2FA.", null, TwoFAVerificationError.NotInitialized);

            var secret = _encryption.Decrypt(user.TwoFactorSecretEncrypted);

            bool ok = _twoFactorDomain.VerifyCode(secret, code);

            if (!ok)
            {
                RegisterFailure(LoginAttempts, userId);
                if (IsRateLimited(LoginAttempts, userId, out var retry))
                    return new(false, BuildRateLimitMessage(retry), null, TwoFAVerificationError.RateLimited);
                return new(false, "Invalid code. Please try again.", null, TwoFAVerificationError.InvalidCode);
            }

            ResetAttempts(LoginAttempts, userId);

            return new(true, "Login successful.");
        }

        private static bool IsCodeFormatValid(string code, out string error)
        {
            error = string.Empty;

            if (string.IsNullOrWhiteSpace(code))
            {
                error = "Two-factor code is required.";
                return false;
            }

            var digits = new string(code.Where(char.IsDigit).ToArray());
            if (digits.Length != 6)
            {
                error = "Enter a 6-digit verification code.";
                return false;
            }

            return true;
        }

        private static bool IsRateLimited(
            ConcurrentDictionary<string, AttemptWindow> store,
            string userId,
            out TimeSpan retryAfter)
        {
            retryAfter = TimeSpan.Zero;
            if (!store.TryGetValue(userId, out var window))
                return false;

            var now = DateTimeOffset.UtcNow;
            if (now - window.WindowStart > AttemptWindowDuration)
            {
                store.TryRemove(userId, out _);
                return false;
            }

            if (window.AttemptCount < MaxAttempts)
                return false;

            retryAfter = window.WindowStart + AttemptWindowDuration - now;
            return true;
        }

        private static void RegisterFailure(
            ConcurrentDictionary<string, AttemptWindow> store,
            string userId)
        {
            var now = DateTimeOffset.UtcNow;

            store.AddOrUpdate(
                userId,
                _ => new AttemptWindow(now, 1),
                (_, existing) =>
                {
                    if (now - existing.WindowStart > AttemptWindowDuration)
                        return new AttemptWindow(now, 1);

                    return new AttemptWindow(existing.WindowStart, existing.AttemptCount + 1);
                });
        }

        private static void ResetAttempts(
            ConcurrentDictionary<string, AttemptWindow> store,
            string userId)
        {
            store.TryRemove(userId, out _);
        }

        private static string BuildRateLimitMessage(TimeSpan retryAfter)
        {
            var seconds = Math.Max(1, (int)Math.Ceiling(retryAfter.TotalSeconds));
            return $"Too many invalid 2FA attempts. Try again in {seconds} seconds.";
        }

        private static IReadOnlyCollection<string> GenerateRecoveryCodes(int count = 8)
        {
            var codes = new List<string>(count);
            for (var i = 0; i < count; i++)
            {
                codes.Add(GenerateRecoveryCode());
            }

            return codes;
        }

        private static string GenerateRecoveryCode()
        {
            Span<byte> buffer = stackalloc byte[5];
            RandomNumberGenerator.Fill(buffer);
            return Convert.ToHexString(buffer).ToLowerInvariant();
        }

        private static IReadOnlyCollection<string> HashRecoveryCodes(IEnumerable<string> recoveryCodes)
        {
            using var sha = SHA256.Create();
            return recoveryCodes
                .Select(code =>
                {
                    var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(code));
                    return Convert.ToHexString(bytes);
                })
                .ToArray();
        }

        private sealed record AttemptWindow(DateTimeOffset WindowStart, int AttemptCount);
    }
}
