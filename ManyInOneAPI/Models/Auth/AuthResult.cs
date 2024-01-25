namespace ManyInOneAPI.Models.Auth
{
    public class AuthResult
    {
        //public string Token { get; set; } = string.Empty; // not needed
        public bool Result { get; set; }
        //public string? UserEmail { get; set; }
        public string? UserId { get; set; } 
        public string? Message { get; set; } 
        public List<string>? Errors { get; set; }
        public bool TwoFAEnabled { get; set; } = false;
        public bool emailConfirmed { get; set; } = false;
    }
}
