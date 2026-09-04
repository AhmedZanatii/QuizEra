using QuizEra.DAL.Entities.Enums;
namespace QuizEra.DAL.Entities
{
    public class Complaint : BaseEntity
    {
        public int Id { get; private set; }
        public int ExamAttemptId { get; private set; }
        public int ExamQuestionId { get; private set; }
        public string Comment { get; private set; }
        public string? Response { get; private set; } = null;
        public ComplaintStatus Status { get; private set; } = ComplaintStatus.Pending;

        // Navigation Properties
        public StudentExamAttempt ExamAttempt { get; private set; }
        public ExamQuestions ExamQuestion { get; private set; }

        protected Complaint() { }

        public Complaint(
            int attemptId, 
            int questionId, 
            string comment, 
            string creatorUser) 
            : base(creatorUser)
        {
            ExamAttemptId = attemptId;
            ExamQuestionId = questionId;
            Comment = comment;
        }

        public void UpdateComment(
            string comment, 
            string modifierUser)
        {
            Comment = comment;
            base.Update(modifierUser);
        }

        public void UpdateResponse(
            string response,
            ComplaintStatus status, 
            string modifierUser)
        {
            Response = response;
            Status = status;
            base.Update(modifierUser);
        }
    }
}