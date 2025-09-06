using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;


namespace Learnova.Infrastructure.Email
{
    public class EmailService(IOptions<MailSettings> mailSettings, ILogger<EmailService> logger) : IEmailSender
    {

        private readonly MailSettings _mailSettings = mailSettings.Value;
        private readonly ILogger<EmailService> _logger = logger;

        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {


            var message = new MimeMessage();

            message.From.Add(new MailboxAddress(_mailSettings.DisplayName, _mailSettings.Mail));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = subject;

            var builder = new BodyBuilder
            {
                HtmlBody = htmlMessage
            };

            message.Body = builder.ToMessageBody();


            using var smtp = new SmtpClient();

            try
            {
                _logger.LogInformation("Sending email to {email}", email);

                await smtp.ConnectAsync(_mailSettings.Host, _mailSettings.Port, SecureSocketOptions.StartTls);
                await smtp.AuthenticateAsync(_mailSettings.Mail, _mailSettings.Password);
                await smtp.SendAsync(message);
                await smtp.DisconnectAsync(true);

                _logger.LogInformation("Email sent successfully to {email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {email}", email);
                throw;
            }
        }

    }
}
