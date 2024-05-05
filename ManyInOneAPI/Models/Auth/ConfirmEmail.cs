namespace ManyInOneAPI.Models.Auth
{
    public class ConfirmEmail
    {
        public required string UserId { get; set; } = string.Empty;
        public required string ConfirmationCode { get; set; } = string.Empty;
    }
}
