using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class StudentExamAttempt
    {
        public int StudExamID { get; private set; }
        public int ExamID { get; private set; }
        public int StudentID { get; private set; }
        public int StudResult { get; private set; }

        // Navigation Properties
        public Exam Exam { get; private set; }
        public Student Student { get; private set; }
        public ICollection<StudentExamQuestionAnswer> StudentExamQuestionAnswers { get; private set; } = new List<StudentExamQuestionAnswer>();

        protected StudentExamAttempt() { }

        public StudentExamAttempt(int examID, int studentID, int studResult)
        {
            ExamID = examID;
            StudentID = studentID;
            StudResult = studResult;
        }

        public void Update(int studResult)
        {
            StudResult = studResult;
        }
    }
}
