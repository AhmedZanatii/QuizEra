using System.Collections.Generic;

namespace QuizEra.DAL.Entities
{
    public class Instructor
    {
        public int InstructorID { get; private set; }

        public string Name { get; private set; }

        public string Email { get; private set; }

        public string AppUserId { get; private set; }

        public ApplicationUser AppUser { get; private set; }

        public ICollection<Course> Courses { get; private set; }
            = new List<Course>();

        protected Instructor()
        {
        }

        public Instructor(
            string name,
            string email,
            string appUserId)
        {
            Name = name;
            Email = email;
            AppUserId = appUserId;
        }

        public void Update(string name, string email)
        {
            Name = name;
            Email = email;
        }
    }
}