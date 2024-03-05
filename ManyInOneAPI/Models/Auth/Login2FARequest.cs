namespace ManyInOneAPI.Models.Auth
{
    public class Login2FARequest
    {
        public required string? CurrUserId { get; set; }
        public required string? TwoFACode { get; set; }
    }
}
