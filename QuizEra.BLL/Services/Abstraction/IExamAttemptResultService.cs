using QuizEra.BLL.ModelVM.ExamAttempt;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IExamAttemptResultService
    {
        Task<ExamAttemptResultVM> GetAttemptResultAsync(int attemptId);
    }
}
