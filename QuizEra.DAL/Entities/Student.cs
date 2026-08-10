using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Student
    {
        public int StudentID { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Email { get; private set; }
        public string Password { get; private set; }

        // Navigation Properties
        public ICollection<StudentCourse> StudentCourses { get; private set; } = new List<StudentCourse>();
        public ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();
        public ICollection<StudentExamAttempt> StudentExamAttempts { get; private set; } = new List<StudentExamAttempt>();

        protected Student() { }

        public Student(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }

        public void Update(string firstName, string lastName, string email, string password)
        {
            FirstName = firstName;
            LastName = lastName;
            Email = email;
            Password = password;
        }
    }
}
