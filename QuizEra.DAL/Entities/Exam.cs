using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Exam
    {
        public int ExamID { get; private set; }
        public int TopicID { get; private set; }
        public string Title { get; private set; }
        public int Duration { get; private set; }
        public int TotalMarks { get; private set; }

        // Navigation Properties
        public Topic Topic { get; private set; }
        public ICollection<ExamQuestions> ExamQuestions { get; private set; } = new List<ExamQuestions>();
        public ICollection<StudentExamAttempt> StudentExamAttempts { get; private set; } = new List<StudentExamAttempt>();

        protected Exam() { } 

        public Exam(int topicID, string title, int duration, int totalMarks)
        {
            TopicID = topicID;
            Title = title;
            Duration = duration;
            TotalMarks = totalMarks;
        }

        public void Update(string title, int duration, int totalMarks, int topicID)
        {
            Title = title;
            Duration = duration;
            TotalMarks = totalMarks;
            TopicID = topicID;
        }
    }
}
