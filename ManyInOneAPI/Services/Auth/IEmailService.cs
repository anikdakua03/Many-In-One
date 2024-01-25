using ManyInOneAPI.Models.Auth;

namespace ManyInOneAPI.Services.Auth
{
    public interface IEmailService
    {
        public Task<AuthResult> SendMailAsync(string userId, string userEmail, string confirmationCode);

        public Task<AuthResult> SendGreetingMailAsync(string userEmail);
    }
}
