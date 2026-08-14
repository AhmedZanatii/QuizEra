using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Course
    {
        public int Id { get; private set; }
        public int InstructorID { get; private set; }
        public string CourseName { get; private set; }
        public string CourseLevel { get; private set; }

        // Navigation Properties
        public Instructor Instructor { get; private set; }
        public ICollection<Topic> Topics { get; private set; } = new List<Topic>();
        public ICollection<StudentCourse> StudentCourses { get; private set; } = new List<StudentCourse>();
        public ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();

        protected Course() { }

        public Course(int instructorID, string courseName, string courseLevel)
        {
            InstructorID = instructorID;
            CourseName = courseName;
            CourseLevel = courseLevel;
        }

        public void Update(int instructorID, string courseName, string courseLevel)
        {
            InstructorID = instructorID;
            CourseName = courseName;
            CourseLevel = courseLevel;
        }
    }
}
