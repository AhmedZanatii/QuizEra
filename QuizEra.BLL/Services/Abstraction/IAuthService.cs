using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using QuizEra.BLL.ModelVM.Auth;
using QuizEra.DAL.Entities;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IAuthService
    {
        Task<IdentityResult> RegisterStudentAsync(RegisterStudentVM model);

        Task<bool> RegisterInstructorAsync(RegisterInstructorVM model);
        Task<bool> ConfirmEmailAsync(string userId, string token);

        Task<LoginResult> LoginAsync(LoginVM model);
        Task<bool> ExternalLoginAsync(
    System.Security.Claims.ClaimsPrincipal principal,
    string email);
        Task LogoutAsync();

        Task<ApplicationUser?> GetUserByEmailAsync(string email);

        Task<string?> GetUserRoleAsync(string userId);

        Task<bool> RegisterExternalUserAsync(
            string email,
            string firstName,
            string lastName,
            string role);

        Task<bool> ForgotPasswordAsync(string email);

        Task<bool> ResetPasswordAsync(ResetPasswordVM model);
    }
}