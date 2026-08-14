namespace QuizEra.DAL.Entities
{
    public class StudentExamQuestionAnswer
    {
        public int ExamQuestionsId { get; private set; }
        public int StudentExamAttemptId { get; private set; }

        public int StudQMarks { get; private set; }
        public string QuestionAnswer { get; private set; }

        // Navigation Properties
        public ExamQuestions ExamQuestions { get; private set; }
        public StudentExamAttempt StudentExamAttempt { get; private set; }

        protected StudentExamQuestionAnswer() { }

        public StudentExamQuestionAnswer(
            int examQuestionsId,
            int studentExamAttemptId,
            int studQMarks,
            string questionAnswer)
        {
            ExamQuestionsId = examQuestionsId;
            StudentExamAttemptId = studentExamAttemptId;
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