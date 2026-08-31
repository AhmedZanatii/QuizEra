using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.StudentExamQuestionAnswer
{
    public class StudentExamQuestionAnswerVM
    {
        public int StudentExamAttemptId { get; set; }
        public int ExamQuestionId { get; set; }

        public string QuestionAnswer { get; set; }
        public int StudQMarks { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public bool IsCorrect { get; set; } = false;
    }
}