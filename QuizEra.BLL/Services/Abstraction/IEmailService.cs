using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IEmailService
    {
        Task SendEmailConfirmationAsync(string email,string confirmationLink);
    }
}
