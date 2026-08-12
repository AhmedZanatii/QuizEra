using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;

namespace QuizEra.DAL.Entities
{
    public class Student
    {
        public int StudentID { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }

        // Identity (1-to-1)
        public string AppUserId { get; private set; }
        public ApplicationUser AppUser { get; private set; }

        // Navigation Properties
        public ICollection<StudentCourse> StudentCourses { get; private set; } = new List<StudentCourse>();
        public ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();
        public ICollection<StudentExamAttempt> StudentExamAttempts { get; private set; } = new List<StudentExamAttempt>();

        protected Student() { }

        public Student(string appUserId, string firstName, string lastName)
        {
            AppUserId = appUserId;
            FirstName = firstName;
            LastName = lastName;
        }

        public void Update(string firstName, string lastName)
        {
            FirstName = firstName;
            LastName = lastName;
        }
    }
}
