using QuizEra.BLL.ModelVM.Notification;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface INotificationService
    {
        Task CreateAndBroadcastExamNotificationAsync(int courseId, string courseName, string examTitle, int examId);
        Task<List<NotificationVM>> GetUserNotificationsAsync(string appUserId);
        Task<bool> MarkAllAsReadAsync(string appUserId);
    }
}
