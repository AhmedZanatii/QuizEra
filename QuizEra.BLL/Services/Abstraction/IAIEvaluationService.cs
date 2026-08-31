public interface IAIEvaluationService
{
    Task<(int Grade, string Justification, bool IsCorrect)> EvaluateEssayAsync(
        string question, string correct, string student, int maxMarks);
}