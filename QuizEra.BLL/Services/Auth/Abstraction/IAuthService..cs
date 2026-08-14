using Microsoft.AspNetCore.Identity;
using QuizEra.BLL.ModelVM.Auth;

namespace QuizEra.BLL.Services.Auth.Abstraction
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterStudentAsync(RegisterStudentVM model);

        Task<bool> RegisterInstructorAsync(RegisterInstructorVM model);

        Task<bool> LoginAsync(LoginVM model);
        Task LogoutAsync();
    }
}