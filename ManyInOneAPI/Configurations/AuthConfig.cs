namespace ManyInOneAPI.Configurations
{
    public class AuthConfig
    {
        public string? Secret { get; set; }

        public string? Audience { get; set; }
        // for google Auth
        public string? GoogleClientId { get; set; }
    }
}
