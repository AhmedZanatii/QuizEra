using QuizEra.BLL.ModelVM.Complaint;
using QuizEra.DAL.Entities.Enums;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IComplaintService
    {
        Task<bool> CreateAsync(ComplaintVM complaintVM, string creatorUser);

        Task<IEnumerable<ComplaintVM>> GetAllByExamIdAsync(int id);

        Task<IEnumerable<ComplaintVM>> GetAllByStudentIdAsync(string id);

        Task<ComplaintVM?> GetByIdAsync(int id);
        Task<bool> UpdateResponseAsync(int id, string status, string response, string modifierUser);

        Task<bool> UpdateCommentAsync(int id, string comment, string modifierUser);

        Task<bool> DeleteExamAsync(int id, string deleterUser);
    }
}
