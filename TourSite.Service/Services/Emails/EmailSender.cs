using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;
using System.Threading;
using TourSite.Core.Servicies.Contract;

namespace TourSite.Service.Services.Emails
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendAsync(string email, string subject, string htmlBody, CancellationToken cancellationToken = default)
        {
            var fromEmail = _config["EmailSettings:From"];
            if (string.IsNullOrWhiteSpace(fromEmail))
                throw new InvalidOperationException("EmailSettings:From configuration is missing or empty.");

            var fromName = _config["EmailSettings:FromName"];
            var smtpServer = _config["EmailSettings:SmtpServer"];
            if (string.IsNullOrWhiteSpace(smtpServer))
                throw new InvalidOperationException("EmailSettings:SmtpServer configuration is missing or empty.");

            var portStr = _config["EmailSettings:Port"];
            if (string.IsNullOrWhiteSpace(portStr) || !int.TryParse(portStr, out int port) || port <= 0)
                throw new InvalidOperationException("EmailSettings:Port configuration is missing or invalid.");

            var username = _config["EmailSettings:Username"];
            var password = _config["EmailSettings:Password"];
            var enableSsl = bool.Parse(_config["EmailSettings:EnableSsl"] ?? "true");

            var mailMessage = new MailMessage
            {
                From = new MailAddress(fromEmail, fromName),
                Subject = subject,
                Body = htmlBody,
                IsBodyHtml = true
            };
            mailMessage.To.Add(email);

            using var smtp = new SmtpClient(smtpServer, port)
            {
                EnableSsl = enableSsl,
                UseDefaultCredentials = false,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(username, password)
            };

            await smtp.SendMailAsync(mailMessage, cancellationToken);
        }
    }
}



