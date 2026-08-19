namespace QuizEra.DAL.Entities
{
    public class StudentExamQuestionAnswer : BaseEntity
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
            string questionAnswer,
            string creatorUser,
            DateTime createdDate)
        : base(creatorUser)
        {
            ExamQuestionsId = examQuestionsId;
            StudentExamAttemptId = studentExamAttemptId;
            StudQMarks = studQMarks;
            QuestionAnswer = questionAnswer;
        }

        public void Update(int studQMarks, string questionAnswer, string modifierUser, DateTime modifiedDate)
        {
            StudQMarks = studQMarks;
            QuestionAnswer = questionAnswer;
            // Call the base class Update method to update modifierUser and modifiedDate
            base.Update(modifierUser); 
        }
    }
}