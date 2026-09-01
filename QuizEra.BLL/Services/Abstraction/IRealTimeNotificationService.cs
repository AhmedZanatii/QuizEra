using System.Threading.Tasks;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IRealTimeNotificationService
    {
        Task SendExamNotificationToGroupAsync(int courseId, string courseName, string examTitle, int examId);
    }
}