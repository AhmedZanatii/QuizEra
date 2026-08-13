using Microsoft.AspNetCore.Identity;

namespace QuizEra.DAL.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public Student? Student { get; private set; }
        public Instructor? Instructor { get; private set; }
    }
}