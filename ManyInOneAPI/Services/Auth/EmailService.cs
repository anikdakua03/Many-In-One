using MailKit.Net.Smtp;
using MailKit.Security;
using ManyInOneAPI.Configurations;
using ManyInOneAPI.Models.Auth;
using Microsoft.Extensions.Options;
using MimeKit;

namespace ManyInOneAPI.Services.Auth
{
    public class EmailService : IEmailService
    {
        private readonly EmailConfig _emailSettings;
        private readonly IConfiguration _configuration;

        public EmailService(IOptionsMonitor<EmailConfig> optionsMonitor, IConfiguration configuration)
        {
            _emailSettings = optionsMonitor.CurrentValue;
            _configuration = configuration;
        }

        public async Task<AuthResult> SendMailAsync(string userId, string userEmail, string confirmationCode)
        {
            var email = new MimeMessage();
            var subject = "Test mail from my one of the project ";
            var emailBody = GetHtmlBody();

            var callbackURL = $"{_emailSettings.APIURL}auth/ConfirmEmail?userID={userId}&code={confirmationCode}";

            // now encoding the email body bcs in code , there may be # ^ escape char , which may affect the url
            var replacedBody = emailBody.Replace("#URL#", callbackURL);

            // sent this email from that email client
            MailRequest mailRequest = new MailRequest()
            {
                ToMail = userEmail,
                Subject = subject,
                Body = replacedBody
            };

            email.Sender = MailboxAddress.Parse(_emailSettings.FromEmail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToMail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            var smtpClient = new SmtpClient();
            // connect and validate
            smtpClient.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtpClient.Authenticate(_emailSettings.FromEmail, _emailSettings.Password);
            // then send
            var res = await smtpClient.SendAsync(email);
            // after that must close the connection
            smtpClient.Disconnect(true);

            if (res != null)
            {
                return new AuthResult()
                {
                    Result = true,
                    Message = "Confirmation mail sent successfully !!"
                };
            }

            return new AuthResult()
            {
                Result = false,
                Message = "Cannot send email !!"
            };
        }


        public async Task<AuthResult> SendGreetingMailAsync(string userEmail)
        {

            // implementation pending
            var email = new MimeMessage();
            var subject = "Test mail from my one of the project ";
            var emailBody = GreetingHtmlBody();

            //var callbackURL = string.Format((_configuration.GetSection("AppSettings:APIURL").Value
                                    //+ _configuration.GetSection("AppSettings:ConfirmEmail").Value),
                                    //userId, confirmationCode);

            // now encoding the email body bcs in code , there may be # ^ escape char , which may affect the url
            //var replacedBody = emailBody.Replace("#URL#", callbackURL);

            // sent this email from that email client
            MailRequest mailRequest = new MailRequest()
            {
                ToMail = userEmail,
                Subject = subject,
                Body = emailBody
            };

            email.Sender = MailboxAddress.Parse(_emailSettings.FromEmail);
            email.To.Add(MailboxAddress.Parse(mailRequest.ToMail));
            email.Subject = mailRequest.Subject;

            var builder = new BodyBuilder();
            builder.HtmlBody = mailRequest.Body;
            email.Body = builder.ToMessageBody();

            var smtpClient = new SmtpClient();
            // connect and validate
            smtpClient.Connect(_emailSettings.Host, _emailSettings.Port, SecureSocketOptions.StartTls);
            smtpClient.Authenticate(_emailSettings.FromEmail, _emailSettings.Password);
            // then send
            var res = await smtpClient.SendAsync(email);
            // after that must close the connection
            smtpClient.Disconnect(true);

            if (res != null)
            {
                return new AuthResult()
                {
                    Result = true,
                    Message = "Welcome mail sent successfully !!"
                };
            }

            return new AuthResult()
            {
                Result = false,
                Message = "Cannot send email !!"
            };
        }

        private string GreetingHtmlBody()
        {
            string emailBody = "<div style=\"text-align: center; vertical-align: middle; height: 100px;\"><h1>Welcome to Many in One</h1>";
            emailBody += "Please confirm your email !!<a href=\"#URL#\"> <strong>Click here</strong> </a>";
            emailBody += "<div style=\"width:100%;background-color:lightblue;text-align:center;margin:10px\">";
            emailBody += "</div></div>";
            return emailBody;
        }
        private string GetHtmlBody()
        {
            string emailBody = "<div style=\"text-align: center; vertical-align: middle; height: 100px;\"><h1>Welcome to Many in One</h1>";
            emailBody += "Please confirm your email !!<a href=\"#URL#\"> <strong>Click here</strong> </a>";
            emailBody += "<div style=\"width:100%;background-color:lightblue;text-align:center;margin:10px\">";
            emailBody += "</div></div>";
            return emailBody;
        }
    }
}
