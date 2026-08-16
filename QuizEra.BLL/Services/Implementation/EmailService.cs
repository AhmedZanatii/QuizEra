using QuizEra.BLL.Services.Abstraction;
using Resend;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Services.Implementation
{
    public class EmailService : IEmailService
    {
        private readonly IResend _resend;

        public EmailService(IResend resend)
        {
            _resend = resend;
        }

        public async Task SendEmailConfirmationAsync(
            string email,
            string confirmationLink)
        {
            var message = new EmailMessage
            {
                From = "QuizEra <onboarding@resend.dev>",
                Subject = "Confirm your email",
                HtmlBody = $"""
                <h2>Welcome!</h2>

                <p>Thank you for registering.</p>

                <p>
                    Please confirm your email address by clicking
                    the button below:
                </p>

                <p>
                    <a href="{confirmationLink}">
                        Confirm Email
                    </a>
                </p>

                <p>If you didn't create this account, you can ignore this email.</p>
                """
            };

            message.To.Add(email);

            await _resend.EmailSendAsync(message);
        }

        public async Task SendPasswordResetEmailAsync(string email,string resetLink)
        {
            var message = new EmailMessage
            {
                From = "QuizEra <onboarding@resend.dev>",
                Subject = "Reset your QuizEra password",
                HtmlBody = $"""
                <h2>Reset Your Password</h2>

                <p>
                    We received a request to reset your QuizEra password.
                </p>

                <p>
                    Click the button below to create a new password:
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

                <p>
                    For security, this link will expire.
                </p>
                """
            };

            message.To.Add(email);

            await _resend.EmailSendAsync(message);
        }
    }


}
