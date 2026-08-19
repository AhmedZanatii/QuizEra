using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Topic : BaseEntity
    {
        public int Id { get; private set; }
        public int CourseID { get; private set; }
        public string Name { get; private set; }

        // Navigation Properties
        public Course Course { get; private set; }
        public ICollection<Exam> Exams { get; private set; } = new List<Exam>();
        public ICollection<Question> Questions { get; private set; } = new List<Question>(); // التعديل الجديد

        protected Topic() { }

        public Topic(int courseID, string name, string creatorUser) : base(creatorUser)
        {
            CourseID = courseID;
            Name = name;
        }

        public void Update(int courseID, string name , string modifierUser)
        {
            CourseID = courseID;
            Name = name;
            base.Update(modifierUser);
        }
    }
}
