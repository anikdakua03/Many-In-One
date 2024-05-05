namespace ManyInOneAPI.Models.Auth
{
    public class AuthResult
    {
        public bool Result { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; } 
        public string? Message { get; set; } 
        public List<string>? Errors { get; set; }
        public bool TwoFAEnabled { get; set; } = false;
        public bool EmailConfirmed { get; set; } = false;
    }
}
