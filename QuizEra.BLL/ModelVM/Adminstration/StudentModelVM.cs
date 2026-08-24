using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.Administration
{
    public class StudentModelVM
    {
        public int Id { get; set; }

        public string? AppUserId { get; set; }

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Phone]
        public string? PhoneNumber { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public bool IsActive { get; set; }
    }
}