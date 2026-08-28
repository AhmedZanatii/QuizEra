using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Exam
    {
        public int Id { get; private set; }

        public string Title { get; private set; }
        public int Duration { get; private set; }
        public double TotalMarks { get; private set; }

        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        public ICollection<ExamTopic> ExamTopics { get; private set; }
            = new List<ExamTopic>();

        public ICollection<ExamQuestions> ExamQuestions { get; private set; }
            = new List<ExamQuestions>();

        public ICollection<StudentExamAttempt> StudentExamAttempts { get; private set; }
            = new List<StudentExamAttempt>();
        protected Exam() { }

        public Exam( string title, int duration, double totalMarks, DateTime startDate, DateTime endDate)
        {
            Title = title;
            Duration = duration;
            TotalMarks = totalMarks;
            StartDate = startDate;
            EndDate = endDate;
        }

        public void Update(string title, int duration, double totalMarks)
        {
            Title = title;
            Duration = duration;
            TotalMarks = totalMarks;
           
        }
        //if instructore needs to extend the end date
        public void Update(string title, int duration, double totalMarks , DateTime startDate, DateTime endDate)
        {
            Title = title;
            Duration = duration;
            TotalMarks = totalMarks;
            StartDate = startDate;
            EndDate = endDate;
        }
    }
}
