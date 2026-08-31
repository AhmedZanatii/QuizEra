using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Feedback : BaseEntity
    {
        public int Id { get; private set; }
        public int StudentID { get; private set; }
        public int CourseID { get; private set; }
        public string Comment { get; private set; }
        public int Rate { get; private set; }

        // Navigation Properties
        public Student Student { get; private set; }
        public Course Course { get; private set; }

        protected Feedback() { }

        public Feedback(int studentID, int courseID, string comment, int rate, 
        string creatorUser) : base(creatorUser)
        {
            StudentID = studentID;
            CourseID = courseID;
            Comment = comment;
            Rate = rate;
        }

        public void Update(string comment, int rate, string modifierUser)
        {
            Comment = comment;
            Rate = rate;
            base.Update(modifierUser);
        }
    }
}
