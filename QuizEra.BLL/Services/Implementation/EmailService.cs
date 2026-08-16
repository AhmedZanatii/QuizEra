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
    }


}
