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
                <!DOCTYPE html>
                <html>
                <head>
                    <meta charset="UTF-8">
                    <meta name="viewport" content="width=device-width, initial-scale=1.0">
                    <title>Confirm your QuizEra email</title>
                </head>

                <body style="
                    margin:0;
                    padding:0;
                    background-color:#f4f6f9;
                    font-family:Arial,Helvetica,sans-serif;
                    color:#212529;
                ">

                    <table width="100%" cellpadding="0" cellspacing="0" border="0"
                           style="background-color:#f4f6f9;padding:40px 15px;">

                        <tr>
                            <td align="center">

                                <table width="100%" cellpadding="0" cellspacing="0" border="0"
                                       style="
                                           max-width:600px;
                                           background:#ffffff;
                                           border-radius:12px;
                                           overflow:hidden;
                                           box-shadow:0 4px 15px rgba(0,0,0,0.08);
                                       ">

                                    <!-- Header -->
                                    <tr>
                                        <td style="
                                            background:#0d6efd;
                                            padding:30px;
                                            text-align:center;
                                        ">
                                            <h1 style="
                                                margin:0;
                                                color:#ffffff;
                                                font-size:28px;
                                            ">
                                                QuizEra
                                            </h1>
                                        </td>
                                    </tr>

                                    <!-- Content -->
                                    <tr>
                                        <td style="padding:40px 35px;">

                                            <h2 style="
                                                margin:0 0 20px;
                                                font-size:24px;
                                                color:#212529;
                                            ">
                                                Welcome to QuizEra!
                                            </h2>

                                            <p style="
                                                margin:0 0 15px;
                                                font-size:16px;
                                                line-height:1.6;
                                                color:#555555;
                                            ">
                                                Thank you for registering with QuizEra.
                                            </p>

                                            <p style="
                                                margin:0 0 25px;
                                                font-size:16px;
                                                line-height:1.6;
                                                color:#555555;
                                            ">
                                                Please confirm your email address
                                                by clicking the button below.
                                            </p>

                                            <!-- Button -->
                                            <table cellpadding="0" cellspacing="0" border="0"
                                                   style="margin:0 auto 30px;">

                                                <tr>
                                                    <td style="
                                                        background:#0d6efd;
                                                        border-radius:8px;
                                                        text-align:center;
                                                    ">

                                                        <a href="{confirmationLink}"
                                                           style="
                                                               display:inline-block;
                                                               padding:14px 30px;
                                                               color:#ffffff;
                                                               text-decoration:none;
                                                               font-size:16px;
                                                               font-weight:bold;
                                                           ">
                                                            Confirm Email
                                                        </a>

                                                    </td>
                                                </tr>

                                            </table>

                                            <p style="
                                                margin:0 0 10px;
                                                font-size:14px;
                                                line-height:1.6;
                                                color:#777777;
                                            ">
                                                If the button doesn't work, copy and paste
                                                the following link into your browser:
                                            </p>

                                            <p style="
                                                word-break:break-all;
                                                font-size:13px;
                                                color:#0d6efd;
                                            ">
                                                {confirmationLink}
                                            </p>

                                            <hr style="
                                                border:0;
                                                border-top:1px solid #eeeeee;
                                                margin:30px 0;
                                            ">

                                            <p style="
                                                margin:0;
                                                font-size:13px;
                                                line-height:1.5;
                                                color:#999999;
                                            ">
                                                If you didn't create this account,
                                                you can safely ignore this email.
                                            </p>

                                        </td>
                                    </tr>

                                    <!-- Footer -->
                                    <tr>
                                        <td style="
                                            background:#f8f9fa;
                                            padding:20px;
                                            text-align:center;
                                        ">

                                            <p style="
                                                margin:0;
                                                font-size:12px;
                                                color:#999999;
                                            ">
                                                © 2026 QuizEra. All rights reserved.
                                            </p>

                                        </td>
                                    </tr>

                                </table>

                            </td>
                        </tr>

                    </table>

                </body>
                </html>
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
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <meta charset="UTF-8">
                        <meta name="viewport" content="width=device-width, initial-scale=1.0">
                        <title>Reset your QuizEra password</title>
                    </head>

                    <body style="
                        margin:0;
                        padding:0;
                        background-color:#f4f6f9;
                        font-family:Arial,Helvetica,sans-serif;
                        color:#212529;
                    ">

                        <table width="100%" cellpadding="0" cellspacing="0" border="0"
                               style="background-color:#f4f6f9;padding:40px 15px;">

                            <tr>
                                <td align="center">

                                    <table width="100%" cellpadding="0" cellspacing="0" border="0"
                                           style="
                                               max-width:600px;
                                               background:#ffffff;
                                               border-radius:12px;
                                               overflow:hidden;
                                               box-shadow:0 4px 15px rgba(0,0,0,0.08);
                                           ">

                                        <!-- Header -->
                                        <tr>
                                            <td style="
                                                background:#0d6efd;
                                                padding:30px;
                                                text-align:center;
                                            ">

                                                <h1 style="
                                                    margin:0;
                                                    color:#ffffff;
                                                    font-size:28px;
                                                ">
                                                    QuizEra
                                                </h1>

                                            </td>
                                        </tr>

                                        <!-- Content -->
                                        <tr>
                                            <td style="padding:40px 35px;">

                                                <h2 style="
                                                    margin:0 0 20px;
                                                    font-size:24px;
                                                    color:#212529;
                                                ">
                                                    Reset Your Password
                                                </h2>

                                                <p style="
                                                    margin:0 0 15px;
                                                    font-size:16px;
                                                    line-height:1.6;
                                                    color:#555555;
                                                ">
                                                    We received a request to reset your
                                                    QuizEra account password.
                                                </p>

                                                <p style="
                                                    margin:0 0 25px;
                                                    font-size:16px;
                                                    line-height:1.6;
                                                    color:#555555;
                                                ">
                                                    Click the button below to create a
                                                    new password.
                                                </p>

                                                <!-- Button -->
                                                <table cellpadding="0" cellspacing="0" border="0"
                                                       style="margin:0 auto 30px;">

                                                    <tr>
                                                        <td style="
                                                            background:#0d6efd;
                                                            border-radius:8px;
                                                            text-align:center;
                                                        ">

                                                            <a href="{resetLink}"
                                                               style="
                                                                   display:inline-block;
                                                                   padding:14px 30px;
                                                                   color:#ffffff;
                                                                   text-decoration:none;
                                                                   font-size:16px;
                                                                   font-weight:bold;
                                                               ">
                                                                Reset Password
                                                            </a>

                                                        </td>
                                                    </tr>

                                                </table>

                                                <p style="
                                                    margin:0 0 10px;
                                                    font-size:14px;
                                                    line-height:1.6;
                                                    color:#777777;
                                                ">
                                                    If the button doesn't work, copy and paste
                                                    this link into your browser:
                                                </p>

                                                <p style="
                                                    word-break:break-all;
                                                    font-size:13px;
                                                    color:#0d6efd;
                                                ">
                                                    {resetLink}
                                                </p>

                                                <hr style="
                                                    border:0;
                                                    border-top:1px solid #eeeeee;
                                                    margin:30px 0;
                                                ">

                                                <p style="
                                                    margin:0;
                                                    font-size:13px;
                                                    line-height:1.5;
                                                    color:#999999;
                                                ">
                                                    If you didn't request a password reset,
                                                    you can safely ignore this email.
                                                    Your password will remain unchanged.
                                                </p>

                                            </td>
                                        </tr>

                                        <!-- Footer -->
                                        <tr>
                                            <td style="
                                                background:#f8f9fa;
                                                padding:20px;
                                                text-align:center;
                                            ">

                                                <p style="
                                                    margin:0;
                                                    font-size:12px;
                                                    color:#999999;
                                                ">
                                                    © 2026 QuizEra. All rights reserved.
                                                </p>

                                            </td>
                                        </tr>

                                    </table>

                                </td>
                            </tr>

                        </table>

                    </body>
                    </html>
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