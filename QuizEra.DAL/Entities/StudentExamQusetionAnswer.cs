namespace QuizEra.DAL.Entities
{
    public class StudentExamQuestionAnswer : BaseEntity
    {
        public int ExamQuestionsId { get; private set; }
        public int StudentExamAttemptId { get; private set; }

        public int StudQMarks { get; private set; }
        public string QuestionAnswer { get; private set; }
        public string? AIJustification { get; private set; }

        public TimeSpan TimeSpent { get; private set; }
        public bool IsCorrect { get; private set; } = false;

        // Navigation Properties
        public ExamQuestions ExamQuestions { get; private set; }
        public StudentExamAttempt StudentExamAttempt { get; private set; }

        protected StudentExamQuestionAnswer() { }

        public StudentExamQuestionAnswer(
            int examQuestionsId,
            int studentExamAttemptId,
            int studQMarks,
            string questionAnswer,
            string creatorUser,
            bool isCorrect,
            TimeSpan timeSpent,
            string? AIJustification = null)
        : base(creatorUser)
        {
            ExamQuestionsId = examQuestionsId;
            StudentExamAttemptId = studentExamAttemptId;
            StudQMarks = studQMarks;
            QuestionAnswer = questionAnswer;
            TimeSpent = timeSpent;
            IsCorrect = isCorrect;
            this.AIJustification = AIJustification;
        }

        public void Update(int studQMarks, string questionAnswer, string modifierUser, 
                            bool isCorrect, TimeSpan timeSpent, string? AIJustification = null)
        {
            StudQMarks = studQMarks;
            QuestionAnswer = questionAnswer;
            TimeSpent = timeSpent;
            IsCorrect = isCorrect;
            this.AIJustification = AIJustification;
            base.Update(modifierUser); 
        }
    }
}