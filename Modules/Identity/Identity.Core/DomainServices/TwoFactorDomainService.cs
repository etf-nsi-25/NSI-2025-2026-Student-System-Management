using Identity.Core.DomainServices;

namespace Identity.Core.DomainServices
{
    // Domain logika za generisanje TOTP secreta / QR koda i validaciju koda.
    public class TwoFactorDomainService
    {
        private readonly ITotpProvider _totpProvider;

        public TwoFactorDomainService(ITotpProvider totpProvider)
        {
            _totpProvider = totpProvider;
        }

        // Generiše novi secret + otpauth podatke za zadani username.
        public (string ManualEntryKey, string QrCodeBase64, string OtpAuthUri) GenerateSetupFor(string username)
        {
            var secret = _totpProvider.GenerateSecret();
            var artifacts = _totpProvider.GenerateSetupArtifacts(username, secret);
            return (secret, artifacts.QrCodeBase64, artifacts.OtpAuthUri);
        }

        public bool VerifyCode(string secret, string code)
        {
            return _totpProvider.ValidateCode(secret, code);
        }
    }
}
