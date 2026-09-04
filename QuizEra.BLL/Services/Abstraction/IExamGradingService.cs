namespace QuizEra.BLL.Services.Abstraction
{
    public interface IExamGradingService
    {
        Task GradeAttemptAsync(int attemptId, string modifierUser);
        Task UpdateStudentAnswerGrade(int attemptId, int questionId, int newGrade, string modifierUser);
    }
}