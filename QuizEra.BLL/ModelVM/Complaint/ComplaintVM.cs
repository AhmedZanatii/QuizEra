using QuizEra.BLL.ModelVM.Exam;
using QuizEra.BLL.ModelVM.StudentExamAttempt;
using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.Complaint
{
    public class ComplaintVM
    {
        public int Id { get; set; }
        public int ExamAttemptId { get; set; }
        public int ExamQuestionId { get; set; }
        public string UserStudentId { get; set; }
        public string StudentAnswer { get; set; }

        [Required(ErrorMessage = "Comment is required.")]
        [MinLength(2, ErrorMessage = "Comment must contain at least 2 characters.")]
        [MaxLength(1000, ErrorMessage = "Comment cannot exceed 1000 characters.")]
        public string Comment { get; set; }
        public string? Response { get; set; } = null;
        public string Status { get; set; } = "Pending";
        public int CurrentMark { get; set; }

        // Navigation Properties
        public CreateExamQuestionVM ExamQuestion { get; set; }
        public StudentExamAttemptVM ExamAttempt { get; set; }
    }
}