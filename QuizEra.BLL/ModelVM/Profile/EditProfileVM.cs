
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.Profile
{
    public class EditProfileVM
    {
        [Required]
        public string FirstName { get; set; }

        [Required]
        public string LastName { get; set; }

        [Phone]
        public string? PhoneNumber { get; set; }

        public string? CurrentProfileImage { get; set; }

        public IFormFile? ProfileImage { get; set; }
    }
}
