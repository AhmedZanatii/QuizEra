using QuizEra.BLL.ModelVM.StudentExamQuestionAnswer;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IStudentQuestionAnswerService
    {
        Task<IEnumerable<StudentExamQuestionAnswerVM>> GetByExamAttemptIdAsync(int id);
        Task<IEnumerable<StudentExamQuestionAnswerVM>> GetByExamQuestionIdAsync(int id);
        Task AddAsync(StudentExamQuestionAnswerVM answer, string creatorUser);
        Task UpdateAsync(StudentExamQuestionAnswerVM answer, string modifierUser);
        Task DeleteAsync(int examQuestionId, int studentExamAttemptId, string deleterUser);
    }
}