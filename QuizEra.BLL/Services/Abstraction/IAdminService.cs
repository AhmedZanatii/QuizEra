using QuizEra.BLL.ModelVM.Administration;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IAdminService
    {
        Task<AdminDashboardVM> GetDashboardAsync();
    }
}