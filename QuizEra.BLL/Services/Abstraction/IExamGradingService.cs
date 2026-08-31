namespace QuizEra.BLL.Services.Abstraction
{
    public interface IExamGradingService
    {
        Task GradeAttemptAsync(int attemptId, string modifierUser);
    }
}