using System.ComponentModel.DataAnnotations;
using QuizEra.BLL.ModelVM.StudentExamQuestionAnswer;

namespace QuizEra.BLL.ModelVM.StudentExamAttempt
{
    public class StudentExamAttemptVM
    {
        public int AttemptId { get; set; }

        [Required]
        public int ExamId { get; set; }
        [Required]
        public int StudentId { get; set; }
        [Range(0, 100)]
        public int StudResult { get; set; }

        public int ShuffleSeed { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public ICollection<StudentExamQuestionAnswerVM> StudentExamQuestionAnswers
        { get; set; } = new List<StudentExamQuestionAnswerVM>();
    }
}