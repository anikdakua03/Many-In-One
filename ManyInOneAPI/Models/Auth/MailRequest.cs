namespace ManyInOneAPI.Models.Auth
{
    public class MailRequest
    {
        public string? ToMail { get; set; }
        public string? Subject { get; set; }
        public string? Body { get; set; }
    }
}
