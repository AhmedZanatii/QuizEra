using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using QuizEra.BLL.Services.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailConfirmationAsync(
            string email,
            string confirmationLink)
        {
            var senderEmail =
                _configuration["EmailSettings:Email"];

            var appPassword =
                _configuration["EmailSettings:AppPassword"];

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    "QuizEra",
                    senderEmail));

            message.To.Add(
                new MailboxAddress(
                    "",
                    email));

            message.Subject = "Confirm your QuizEra email";

            message.Body = new BodyBuilder
            {
                HtmlBody = $"""
                    <h2>Welcome to QuizEra!</h2>

                    <p>Thank you for registering.</p>

                    <p>
                        Please confirm your email address
                        by clicking the button below:
                    </p>

                    <p>
                        <a href="{confirmationLink}">
                            Confirm Email
                        </a>
                    </p>

                    <p>
                        If you didn't create this account,
                        you can ignore this email.
                    </p>
                    """
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                senderEmail,
                appPassword);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }


        public async Task SendPasswordResetEmailAsync(
            string email,
            string resetLink)
        {
            var senderEmail =
                _configuration["EmailSettings:Email"];

            var appPassword =
                _configuration["EmailSettings:AppPassword"];

            var message = new MimeMessage();

            message.From.Add(
                new MailboxAddress(
                    "QuizEra",
                    senderEmail));

            message.To.Add(
                new MailboxAddress(
                    "",
                    email));

            message.Subject = "Reset your QuizEra password";

            message.Body = new BodyBuilder
            {
                HtmlBody = $"""
                    <h2>Reset Your Password</h2>

                    <p>
                        We received a request to reset
                        your QuizEra password.
                    </p>

                    <p>
                        Click the button below:
                    </p>

                    <p>
                        <a href="{resetLink}">
                            Reset Password
                        </a>
                    </p>

                    <p>
                        If you didn't request a password reset,
                        you can safely ignore this email.
                    </p>
                    """
            }.ToMessageBody();

            using var smtp = new SmtpClient();

            await smtp.ConnectAsync(
                "smtp.gmail.com",
                587,
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                senderEmail,
                appPassword);

            await smtp.SendAsync(message);

            await smtp.DisconnectAsync(true);
        }
    }
}