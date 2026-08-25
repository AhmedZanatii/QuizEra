using Microsoft.AspNetCore.Identity;

namespace QuizEra.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public bool IsActive { get; set; } = true;

        public Student Student { get; set; }

        public Instructor Instructor { get; set; }
    }
}