namespace ManyInOneAPI.Models.Auth
{
    public class TwoFAResponse
    {
        public string? QR { get; set; }
        public string? SharedKey { get; set; }
        public string? Message { get; set; }
        public bool Result { get; set; }
    }
}
