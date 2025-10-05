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
    var client = new SendGridClient(_config["SendGrid:ApiKey"]);
    var from = new EmailAddress("g6447915@gmail.com", "Your App Name");
    var to = new EmailAddress(email);
    var msg = MailHelper.CreateSingleEmail(from, to, subject, "", htmlMessage);

    var response = await client.SendEmailAsync(msg);
    Console.WriteLine($"SendGrid status: {response.StatusCode}");
    var body = await response.Body.ReadAsStringAsync();
    Console.WriteLine($"SendGrid response: {body}");
}

    }
}