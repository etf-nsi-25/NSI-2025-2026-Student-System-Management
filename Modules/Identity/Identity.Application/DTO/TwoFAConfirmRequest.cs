namespace Identity.Application.DTO
{
    public class TwoFAConfirmRequest
    {
        public string UserId { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
    }
}
