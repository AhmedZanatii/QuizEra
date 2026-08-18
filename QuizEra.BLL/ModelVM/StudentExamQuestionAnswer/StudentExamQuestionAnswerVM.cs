using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.StudentExamQuestionAnswer
{
    public class StudentExamQuestionAnswerVM
    {
        [Required(ErrorMessage = "Student Exam Attempt ID is required")]
        public int StudentExamAttemptId { get; set; }

        [Required(ErrorMessage = "Exam Question ID is required")]
        public int ExamQuestionId { get; set; }

        [Required(ErrorMessage = "Answer is required")]
        [MinLength(1, ErrorMessage = "Answer Min length is 1 :(")]
        public string QuestionAnswer { get; set; }
    }
}