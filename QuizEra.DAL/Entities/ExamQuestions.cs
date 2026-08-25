namespace QuizEra.DAL.Entities
{
    public class ExamQuestions
    {
        public int Id { get; private set; }

        public int QuestionId { get; private set; }
        public int ExamId { get; private set; }

        public double ActualMark { get; private set; }
        public double BonusMark { get; private set; }
        public double NegativeMark { get; private set; }

        // Navigation Properties
        public Question Question { get; private set; }
        public Exam Exam { get; private set; }

        public ICollection<StudentExamQuestionAnswer> StudentExamQuestionAnswers
        { get; private set; } = new List<StudentExamQuestionAnswer>();

        protected ExamQuestions() { }

        public ExamQuestions(int questionId, int examId, double actualMark, double bonusMark,double negativeMarks)
        {
            QuestionId = questionId;
            ExamId = examId;
            ActualMark = actualMark;
            BonusMark = bonusMark;
            NegativeMark = negativeMarks;
        }

        public void Update(double actualMark, double bonusMark, double negativeMarks)
        {
            ActualMark = actualMark;
            BonusMark = bonusMark;
            NegativeMark = negativeMarks;
        }
    }
}