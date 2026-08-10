using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Topic
    {
        public int TopicID { get; private set; }
        public int CourseID { get; private set; }
        public string Name { get; private set; }

        // Navigation Properties
        public Course Course { get; private set; }
        public ICollection<Exam> Exams { get; private set; } = new List<Exam>();

        protected Topic() { }

        public Topic(int courseID, string name)
        {
            CourseID = courseID;
            Name = name;
        }

        public void Update(string name, int courseID)
        {
            Name = name;
            CourseID = courseID;
        }
    }
}
