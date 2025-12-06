namespace Identity.Application.DTO
{
    public class TwoFASetupResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public string ManualEntryKey { get; set; } = string.Empty;
        public string QrCodeBase64 { get; set; } = string.Empty;
        public string OtpAuthUri { get; set; } = string.Empty;

        public TwoFASetupResult(bool success, string message)
        {
            Success = success;
            Message = message;
        }

        public TwoFASetupResult(
            bool success,
            string message,
            string manualEntryKey,
            string qrCodeBase64,
            string otpAuthUri)
        {
            Success = success;
            Message = message;
            ManualEntryKey = manualEntryKey;
            QrCodeBase64 = qrCodeBase64;
            OtpAuthUri = otpAuthUri;
        }
    }
}
