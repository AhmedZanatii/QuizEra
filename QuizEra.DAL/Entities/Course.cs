using System;
using System.Collections.Generic;

namespace QuizEra.DAL.Entities
{
    public class Course : BaseEntity
    {
        public int Id { get; private set; }
        public int InstructorID { get; private set; }
        public string CourseName { get; private set; } = string.Empty;
        public string CourseLevel { get; private set; } = string.Empty;
        public Guid CourseCode { get; private set; } = Guid.NewGuid();
        public string CourseDescription { get; private set; } = string.Empty;

        // Navigation Properties
        public Instructor Instructor { get; private set; } = null!;
        public ICollection<Topic> Topics { get; private set; } = new List<Topic>();
        public ICollection<StudentCourse> StudentCourses { get; private set; } = new List<StudentCourse>();
        public ICollection<Feedback> Feedbacks { get; private set; } = new List<Feedback>();

        protected Course() { }

        public Course(
            int instructorID, 
            string courseName, 
            string courseLevel, 
            string courseDescription, 
            string creatorUser) 
            : base(creatorUser)
        {
            InstructorID = instructorID;
            CourseName = courseName;
            CourseLevel = courseLevel;
            CourseDescription = courseDescription;
        }

        public void Update(
            string courseName, 
            string courseLevel, 
            string courseDescription, 
            string modifierUser)
        {
            CourseName = courseName;
            CourseLevel = courseLevel;
            CourseDescription = courseDescription;
            base.Update(modifierUser);
        }
        public void ChangeInstructor(int instructorId)
        {
            InstructorID = instructorId;
        }
    }
}