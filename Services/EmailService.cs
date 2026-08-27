using MailKit.Net.Smtp;
using MimeKit;

namespace CosmosCrudApi.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendRegistrationEmailAsync(
            string email,
            string name)
        {
            var message = new MimeMessage();

            string senderName =
                _configuration["EmailSettings:SenderName"]!;

            string senderEmail =
                _configuration["EmailSettings:SenderEmail"]!;

            string username =
                _configuration["EmailSettings:Username"]!;

            string password =
                _configuration["EmailSettings:Password"]!;

            string smtpServer =
                _configuration["EmailSettings:SmtpServer"]!;

            int port =
                int.Parse(_configuration["EmailSettings:Port"]!);

            message.From.Add(
                new MailboxAddress(senderName, senderEmail));

            message.To.Add(
                new MailboxAddress(name, email));

            message.Subject = "Registration Successful";

            message.Body = new TextPart("plain")
            {
                Text =
                    $"Hello {name},\n\n" +
                    "Your registration was successful.\n\n" +
                    "Thank you for registering with us.\n\n" +
                    "Regards,\n" +
                    "Cosmos CRUD Application"
            };

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                smtpServer,
                port,
                MailKit.Security.SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                username,
                password);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}