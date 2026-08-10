using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class StudentExamQuestionAnswer
    {
        public int ExamQID { get; private set; }
        public int StudExamID { get; private set; }
        public int StudQMarks { get; private set; }
        public string QuestionAnswer { get; private set; }

        // Navigation Properties
        public ExamQuestions ExamQuestions { get; private set; }
        public StudentExamAttempt StudentExamAttempt { get; private set; }

        protected StudentExamQuestionAnswer() { }

        public StudentExamQuestionAnswer(int examQID, int studExamID, int studQMarks, string questionAnswer)
        {
            ExamQID = examQID;
            StudExamID = studExamID;
            StudQMarks = studQMarks;
            QuestionAnswer = questionAnswer;
        }

        public void Update(int studQMarks, string questionAnswer)
        {
            StudQMarks = studQMarks;
            QuestionAnswer = questionAnswer;
        }
    }
}
