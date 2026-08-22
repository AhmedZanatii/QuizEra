namespace QuizEra.DAL.Entities
{
    public class StudentExamQuestionAnswer : BaseEntity
    {
        public int ExamQuestionsId { get; private set; }
        public int StudentExamAttemptId { get; private set; }

        public int StudQMarks { get; private set; }
        public string QuestionAnswer { get; private set; }

        public TimeSpan TimeSpent { get; private set; }

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
            DateTime createdDate,
            TimeSpan timeSpent)
        : base(creatorUser)
        {
            ExamQuestionsId = examQuestionsId;
            StudentExamAttemptId = studentExamAttemptId;
            StudQMarks = studQMarks;
            QuestionAnswer = questionAnswer;
            TimeSpent = timeSpent;
        }

        public void Update(int studQMarks, string questionAnswer, string modifierUser, DateTime modifiedDate, TimeSpan timeSpent)
        {
            StudQMarks = studQMarks;
            QuestionAnswer = questionAnswer;
            TimeSpent = timeSpent;
            // Call the base class Update method to update modifierUser and modifiedDate
            base.Update(modifierUser); 
        }
    }
}