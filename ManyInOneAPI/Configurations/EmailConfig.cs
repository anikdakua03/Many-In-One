namespace ManyInOneAPI.Configurations
{
    public class EmailConfig
    {
        public string? FromEmail { get; set; }
        public string? Password { get; set; }
        public string? Host { get; set; }
        public string? DisplayName { get; set; }
        public int Port { get; set; }

        // this is for setting up confirm email link
        public string? APIURL { get; set; } //": "https://localhost:8081/api/",
    }
}
