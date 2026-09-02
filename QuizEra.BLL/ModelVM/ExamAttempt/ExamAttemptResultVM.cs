namespace QuizEra.BLL.ModelVM.ExamAttempt
{
    public class ExamAttemptResultVM
    {
        public int AttemptId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int TotalScore { get; set; }
        public decimal Percentage { get; set; }
        public bool IsPassed { get; set; }
        public TimeSpan? CompletionTime { get; set; }
        public int CorrectAnswersCount { get; set; }
        public int IncorrectAnswersCount { get; set; }
        public List<QuestionResultDetailVM> QuestionBreakdown { get; set; } = new();
    }

    public class QuestionResultDetailVM
    {
        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionFormat { get; set; } = string.Empty;
        public string CorrectAnswer { get; set; } = string.Empty;
        public string StudentAnswer { get; set; } = string.Empty;
        public int QuestionMark { get; set; }
        public string? AIJustification { get; set; }
        public bool IsCorrect { get; set; }
        public TimeSpan TimeSpent { get; set; }
        public List<OptionResultVM> Options { get; set; } = new();
    }

    public class OptionResultVM
    {
        public string OptionText { get; set; } = string.Empty;
        public bool IsCorrect { get; set; }
    }
}
