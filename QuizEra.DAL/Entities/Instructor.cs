using System.Collections.Generic;

namespace QuizEra.DAL.Entities
{
    public class Instructor
    {
        public int Id { get; private set; }
        public string AppUserId { get; private set; }

        public ApplicationUser AppUser { get; private set; }

        public ICollection<Course> Courses { get; private set; }
            = new List<Course>();

        protected Instructor()
        {
        }

        public Instructor(string appUserId)
        {
            AppUserId = appUserId;
        }
    }
}