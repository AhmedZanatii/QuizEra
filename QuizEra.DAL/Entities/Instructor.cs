using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Instructor
    {
        public int InstructorID { get; private set; }
        public string Name { get; private set; }

        // Identity (1-to-1)
        public string AppUserId { get; private set; }
        public ApplicationUser AppUser { get; private set; }

        // Navigation Property
        public ICollection<Course> Courses { get; private set; } = new List<Course>();

        protected Instructor() { }

        public Instructor(string appUserId, string name)
        {
            AppUserId = appUserId;
            Name = name;
        }
        public void Update(string name)
        {
            Name = name;
        }
    }
}

