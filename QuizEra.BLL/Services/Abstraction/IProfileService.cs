using QuizEra.BLL.ModelVM.Profile;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IProfileService
    {
        Task<ProfileVM?> GetProfileAsync(string userId);

        Task<EditProfileVM?> GetEditProfileAsync(string userId);

        Task<bool> UpdateProfileAsync(
            string userId,
            EditProfileVM model,
            string? profileImagePath);
    }
}