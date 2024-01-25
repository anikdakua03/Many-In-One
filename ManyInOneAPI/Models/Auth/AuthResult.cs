namespace ManyInOneAPI.Models.Auth
{
    public class AuthResult
    {
        public string Token { get; set; } = string.Empty;
        public bool Result { get; set; }
        public string? UserEmail { get; set; }
        public List<string>? Errors { get; set; }
    }
}
