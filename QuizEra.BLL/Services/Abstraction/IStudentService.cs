using QuizEra.BLL.ModelVM.Administration;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IStudentService
    {
        Task<IEnumerable<StudentModelVM>> GetAllAsync();

        Task<StudentModelVM?> GetByIdAsync(int id);

        Task<bool> CreateAsync(StudentModelVM vm);

        Task<bool> UpdateAsync(StudentModelVM vm);

        Task<bool> DeactivateAsync(int id);

        Task<bool> RestoreAsync(int id);
    }
}