
using Microsoft.AspNetCore.Identity;
using QuizEra.BLL.ModelVM.Profile;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;

namespace QuizEra.BLL.Services.Implementation
{
    public class ProfileService : IProfileService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public ProfileService(
            UserManager<ApplicationUser> userManager
             )
        {
            _userManager = userManager;
        }

        // =========================
        // Get Profile
        // =========================

        public async Task<ProfileVM?> GetProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return new ProfileVM
            {
                UserId = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                ProfileImage = user.ProfileImage,
                Role = roles.FirstOrDefault() ?? ""
            };
        }

        // =========================
        // Get Edit Profile
        // =========================

        public async Task<EditProfileVM?> GetEditProfileAsync(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            return new EditProfileVM
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                PhoneNumber = user.PhoneNumber,
                CurrentProfileImage = user.ProfileImage
            };
        }

        // =========================
        // Update Profile
        // =========================

        public async Task<bool> UpdateProfileAsync(
            string userId,
            EditProfileVM model,
            string? profileImagePath)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            if (!string.IsNullOrEmpty(profileImagePath))
            {
                user.ProfileImage = profileImagePath;
            }

            var result = await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }
    }
}
