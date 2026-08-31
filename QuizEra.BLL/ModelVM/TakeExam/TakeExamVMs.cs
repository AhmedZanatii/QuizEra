namespace QuizEra.Web.ViewModels
{
    public class ExamTakeUIViewModel
    {
        public int ExamId { get; set; }
        public int AttemptId { get; set; }
        public string StudentId { get; set; }
        public string ExamTitle { get; set; }
        public int RemainingTimeSeconds { get; set; }
        
        // This list will be serialized to JSON for the frontend
        public List<QuestionUIModel> Questions { get; set; } = new List<QuestionUIModel>();
    }

    public class QuestionUIModel
    {
        public int QuestionId { get; set; }
        public int ExamQuestionId { get; set; }
        public int QuestionNumber { get; set; }
        public string Text { get; set; }
        
        public string QuestionFormat { get; set; } 

        public List<OptionUIModel> Options { get; set; } = new List<OptionUIModel>();
        
        // State tracking
        public string SelectedAnswer { get; set; } 
        public bool IsFlagged { get; set; }
        public int TimeSpentSeconds { get; set; }
    }

    public class OptionUIModel
    {
        public string Label { get; set; } // "A", "B", "C", "D"
        public string Text { get; set; }
        public string Value { get; set; } = string.Empty;
    }
}