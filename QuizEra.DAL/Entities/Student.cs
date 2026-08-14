using System.Collections.Generic;

namespace QuizEra.DAL.Entities
{
    public class Student
    {
        public int Id { get; private set; }
        public string AppUserId { get; private set; }

        public ApplicationUser AppUser { get; private set; }

        public ICollection<StudentCourse> StudentCourses { get; private set; }
            = new List<StudentCourse>();

        public ICollection<Feedback> Feedbacks { get; private set; }
            = new List<Feedback>();

        public ICollection<StudentExamAttempt> StudentExamAttempts { get; private set; }
            = new List<StudentExamAttempt>();

        protected Student()
        {
        }

        public Student(string appUserId)
        {
            AppUserId = appUserId;
        }
    }
}