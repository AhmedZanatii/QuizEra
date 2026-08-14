namespace QuizEra.DAL.Entities
{
    public class ExamQuestions
    {
        public int Id { get; private set; }

        public int QuestionId { get; private set; }
        public int ExamId { get; private set; }

        public int ActualMark { get; private set; }

        // Navigation Properties
        public Question Question { get; private set; }
        public Exam Exam { get; private set; }

        public ICollection<StudentExamQuestionAnswer> StudentExamQuestionAnswers
        { get; private set; } = new List<StudentExamQuestionAnswer>();

        protected ExamQuestions() { }

        public ExamQuestions(
            int questionId,
            int examId,
            int actualMark)
        {
            QuestionId = questionId;
            ExamId = examId;
            ActualMark = actualMark;
        }

        public void Update(int actualMark)
        {
            ActualMark = actualMark;
        }
    }
}