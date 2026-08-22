using System;
using System.Collections.Generic;

namespace QuizEra.BLL.ModelVM.Analytics
{
    public class StudentAnalyticsVM
    {
        public int AttemptId { get; set; }
        public string StudentName { get; set; }
        public string ExamTitle { get; set; }
        public int TotalScore { get; set; }
        public decimal Percentage { get; set; }
        public bool IsPassed { get; set; }

        public TimeSpan? CompletionTime { get; set; } 

        public int CorrectAnswersCount { get; set; }
        public int IncorrectAnswersCount { get; set; }

        public List<QuestionAnalyticsVM> QuestionBreakdown { get; set; } = new();
    }

    public class QuestionAnalyticsVM
    {
        public string QuestionText { get; set; }
        public bool IsCorrect { get; set; }
        public TimeSpan TimeSpent { get; set; } 
    }
}
