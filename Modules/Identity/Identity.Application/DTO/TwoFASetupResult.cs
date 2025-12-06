namespace Identity.Application.DTO
{
    public class TwoFASetupResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string Secret { get; set; }
        public string QrCode { get; set; }

        public TwoFASetupResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public TwoFASetupResult(bool success, string secret, string qrCode)
        {
            Success = success;
            Secret = secret;
            QrCode = qrCode;
        }
    }
}
