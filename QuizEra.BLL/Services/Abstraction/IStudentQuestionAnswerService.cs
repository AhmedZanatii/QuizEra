using QuizEra.BLL.ModelVM.StudentExamQuestionAnswer;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IStudentQuestionAnswerService
    {
        Task<IEnumerable<StudentExamQuestionAnswerVM>> GetByExamAttemptIdAsync(int id);
        Task<IEnumerable<StudentExamQuestionAnswerVM>> GetByExamQuestionIdAsync(int id);
        Task AddAsync(StudentExamQuestionAnswerVM answer);
        Task UpdateAsync(StudentExamQuestionAnswerVM answer);
    }
}