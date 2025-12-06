using System.Security.Cryptography;
using System.Text;
using Identity.Core.DomainServices;
using OtpNet;
using System.Net; 

namespace Identity.Infrastructure.TOTP
{
    /// <summary>
    /// Full real TOTP implementation using Otp.NET.
    /// Compatible with Google Authenticator, Microsoft Authenticator, Authy…
    /// </summary>
    public class TotpProvider : ITotpProvider
    {
        private const string Issuer = "StudentSystem";

        public string GenerateSecret()
        {
            var bytes = RandomNumberGenerator.GetBytes(20);

            return Base32Encoding.ToString(bytes);
        }

        public string GenerateQrCode(string username, string secret)
        {
            string otpauth =
                $"otpauth://totp/{WebUtility.UrlEncode(Issuer)}:{WebUtility.UrlEncode(username)}" +
                $"?secret={secret}&issuer={WebUtility.UrlEncode(Issuer)}&digits=6";

            return Convert.ToBase64String(Encoding.UTF8.GetBytes(otpauth));
        }

        public bool ValidateCode(string secret, string code)
        {
            if (string.IsNullOrWhiteSpace(secret) || string.IsNullOrWhiteSpace(code))
                return false;

            code = code.Replace(" ", "").Replace("-", "");

            var bytes = Base32Encoding.ToBytes(secret);
            var totp = new Totp(bytes, step: 30, totpSize: 6);

            return totp.VerifyTotp(
                code,
                out long _,
                new VerificationWindow(previous: 1, future: 1)
            );
        }
    }
}
