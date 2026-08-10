using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Instructor
    {
        public int InstructorID { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        // Navigation Property
        public ICollection<Course> Courses { get; private set; } = new List<Course>();

        protected Instructor() { }

        public Instructor(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }

        public void Update(string name, string email, string password)
        {
            Name = name;
            Email = email;
            Password = password;
        }
    }
}
