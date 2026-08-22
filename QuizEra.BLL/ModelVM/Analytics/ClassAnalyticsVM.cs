using System.Collections.Generic;

namespace QuizEra.BLL.ModelVM.Analytics
{
    public class ClassAnalyticsVM
    {
        public int ExamId { get; set; }
        public string ExamTitle { get; set; }

        public double AverageScore { get; set; }
        public int HighestScore { get; set; }
        public int LowestScore { get; set; }

        public Dictionary<string, int> GradeDistribution { get; set; } = new();

        public List<MissedQuestionVM> FrequentlyMissedQuestions { get; set; } = new();
        public List<StudentRankingVM> StudentRankings { get; set; } = new();
    }

    public class MissedQuestionVM
    {
        public string QuestionText { get; set; }
        public int MissCount { get; set; }
    }

    public class StudentRankingVM
    {
        public string StudentName { get; set; }
        public int Score { get; set; }
        public int Rank { get; set; }
    }
}