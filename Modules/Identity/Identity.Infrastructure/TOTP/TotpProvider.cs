using System.Net;
using System.Security.Cryptography;
using System.Text;
using Identity.Core.DomainServices;
using OtpNet;
using QRCoder;

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

        public TotpSetupArtifacts GenerateSetupArtifacts(string username, string secret)
        {
            var otpauth = BuildOtpAuthUri(username, secret);
            var qrBase64 = GenerateQrCodeBase64(otpauth);
            return new TotpSetupArtifacts(otpauth, qrBase64);
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
                out _,
                new VerificationWindow(previous: 2, future: 2)
            );
        }
        private static string BuildOtpAuthUri(string username, string secret)
        {
            var label = $"{Issuer}:{username}".TrimEnd(':');
            return $"otpauth://totp/{WebUtility.UrlEncode(label)}?secret={secret}&issuer={WebUtility.UrlEncode(Issuer)}&digits=6";
        }

        private static string GenerateQrCodeBase64(string otpauth)
        {
            var qrGenerator = new QRCodeGenerator();
            using var qrData = qrGenerator.CreateQrCode(otpauth, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrData);
            var qrBytes = qrCode.GetGraphic(10);
            return $"data:image/png;base64,{Convert.ToBase64String(qrBytes)}";
        }
    }
}
