using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class ExamQuestions
    {
        public int ExamQID { get; private set; }
        public int QuestionID { get; private set; }
        public int ExamID { get; private set; }
        public int ActualMark { get; private set; }

        // Navigation Properties
        public Question Question { get; private set; }
        public Exam Exam { get; private set; }
        public ICollection<StudentExamQuestionAnswer> StudentExamQuestionAnswers { get; private set; } = new List<StudentExamQuestionAnswer>();

        protected ExamQuestions() { }

        public ExamQuestions(int questionID, int examID, int actualMark)
        {
            QuestionID = questionID;
            ExamID = examID;
            ActualMark = actualMark;
        }

        public void Update(int actualMark)
        {
            ActualMark = actualMark;
        }
    }
}
