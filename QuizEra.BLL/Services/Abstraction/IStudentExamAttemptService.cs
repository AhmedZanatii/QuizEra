using QuizEra.BLL.ModelVM.StudentExamAttempt;
using QuizEra.BLL.ModelVM.StudentExamQuestionAnswer;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IStudentExamAttemptService
    {
        Task AddAnswerAsync(StudentExamAttemptVM attempt, StudentExamQuestionAnswerVM answer, string user);
        Task<IEnumerable<StudentExamAttemptVM>> GetByExamIdAsync(int id);
        Task<IEnumerable<StudentExamAttemptVM>> GetByStudentIdAsync(string id);
        Task<StudentExamAttemptVM> GetExactAttemptAsync(int examId, string studentId);
        Task<StudentExamAttemptVM> StartAttemptAsync(int examId, string studentUserId, string creatorUser);
        Task CompleteAttemptAsync(int examId, string studentUserId, string modifierUser);
        Task AddAsync(StudentExamAttemptVM attempt, string creatorUser);
        Task UpdateAsync(StudentExamAttemptVM attempt, string modifierUser);
        Task DeleteAsync(int examId, string studentId, string deleterUser);
    }
}