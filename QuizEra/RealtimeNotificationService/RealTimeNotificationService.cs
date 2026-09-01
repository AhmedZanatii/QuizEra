using Microsoft.AspNetCore.SignalR;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.Hubs;
using System.Threading.Tasks;

namespace QuizEra.RealtimeNotificationService
{
    public class RealTimeNotificationService : IRealTimeNotificationService
    {
        private readonly IHubContext<NotificationHub> _hubContext;

        public RealTimeNotificationService(IHubContext<NotificationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendExamNotificationToGroupAsync(int courseId, string courseName, string examTitle, int examId)
        {
            await _hubContext.Clients.Group($"Course_{courseId}").SendAsync("ReceiveExamNotification", new
            {
                title = "New Exam Posted",
                message = $"A new exam '{examTitle}' has been added to {courseName}.",
                examId = examId,
                courseId = courseId,
                createdAt = "Just now"
            });
        }
    }
}