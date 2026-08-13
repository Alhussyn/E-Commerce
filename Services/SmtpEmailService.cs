using System.Net;
using System.Net.Mail;

namespace E_Commerce.Services
{
    public class SmtpEmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public SmtpEmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendEmailAsync(string toEmail, string subject, string body)
        {
            var smtpSection = _config.GetSection("Smtp");

            var host     = smtpSection["Host"]!;
            var port     = int.Parse(smtpSection["Port"]!);
            var from     = smtpSection["From"]!;
            var username = smtpSection["Username"]!;
            var password = smtpSection["Password"]!;
            var enableSsl = bool.Parse(smtpSection["EnableSsl"] ?? "true");

            using var client = new SmtpClient(host, port)
            {
                Credentials = new NetworkCredential(username, password),
                EnableSsl   = enableSsl
            };

            var message = new MailMessage
            {
                From       = new MailAddress(from, "E-Commerce Store"),
                Subject    = subject,
                Body       = body,
                IsBodyHtml = true
            };
            message.To.Add(toEmail);

            await client.SendMailAsync(message);
        }
    }
}
