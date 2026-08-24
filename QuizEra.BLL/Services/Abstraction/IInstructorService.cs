using QuizEra.BLL.ModelVM.Administration;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IInstructorService
    {
        Task<IEnumerable<InstructorModelVM>> GetAllAsync();

        Task<InstructorModelVM?> GetByIdAsync(int id);

        Task<bool> CreateAsync(InstructorModelVM vm);

        Task<bool> UpdateAsync(InstructorModelVM vm);

        Task<bool> DeactivateAsync(int id);

        Task<bool> RestoreAsync(int id);
    }
}