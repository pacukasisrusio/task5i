using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace penkta.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            var apiKey = _config["SendGrid:ApiKey"];
            var client = new SendGridClient(apiKey);

            var from = new EmailAddress(
                _config["SendGrid:FromEmail"],
                _config["SendGrid:FromName"] ?? "The App");

            var to = new EmailAddress(email);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent: null, htmlContent: htmlMessage);

            var response = await client.SendEmailAsync(msg);

            Console.WriteLine($"[SendGrid] Status: {response.StatusCode}");
        }
    }
}