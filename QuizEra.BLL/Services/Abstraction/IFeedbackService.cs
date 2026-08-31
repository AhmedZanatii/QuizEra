using QuizEra.BLL.ModelVM.Feedback;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IFeedbackService
    {
        Task<IEnumerable<FeedbackVM>> GetByStudentIdAsync(string id);
        Task<IEnumerable<FeedbackVM>> GetByCourseIdAsync(int id);
        Task<FeedbackVM> GetByIdAsync(int id);
        Task AddAsync(FeedbackVM feedback, string creatorUser);
        Task UpdateAsync(FeedbackVM feedback, string modifierUser);
        Task DeleteAsync(int id, string deleterUser);
    }
}