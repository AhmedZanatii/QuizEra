using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class StudentCourse
    {
        public int StudentID { get; private set; }
        public int CourseID { get; private set; }

        // Navigation Properties
        public Student Student { get; private set; }
        public Course Course { get; private set; }

        protected StudentCourse() { }

        public StudentCourse(int studentID, int courseID)
        {
            StudentID = studentID;
            CourseID = courseID;
        }
    }
}
