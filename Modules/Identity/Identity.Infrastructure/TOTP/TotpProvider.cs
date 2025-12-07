using System.Security.Cryptography;
using System.Text;
using Identity.Core.DomainServices;

namespace Identity.Infrastructure.TOTP
{
    /// <summary>
    /// Simple demo implementation – for now accepts code "123456".
    /// Replace with real TOTP algorithm later.
    /// </summary>
    public class TotpProvider : ITotpProvider
    {
        public string GenerateSecret()
        {
            var bytes = RandomNumberGenerator.GetBytes(16);
            var raw = Convert
                .ToBase64String(bytes)
                .Replace("=", string.Empty)
                .Replace("+", string.Empty)
                .Replace("/", string.Empty);

            if (raw.Length < 16)
                raw = raw.PadRight(16, 'A');
            return raw.Substring(0, 16).ToUpperInvariant();
        }

        public string GenerateQrCode(string username, string secret)
        {
            // For real: build otpauth:// URL + generate QR and return Base64.
            // For demo: return empty string so frontend just hides the image
            // or you can return the otpauth URI as text.
            var otpauth =
                $"otpauth://totp/StudentSystem:{username}?secret={secret}&issuer=StudentSystem";
            var bytes = Encoding.UTF8.GetBytes(otpauth);
            return Convert.ToBase64String(bytes); // pseudo "QR" data
        }

        public bool ValidateCode(string secret, string code)
        {
            // DEMO: accept only "123456"
            // Later: plug real TOTP lib and use secret + current time.
            return code == "123456";
        }
    }
}
