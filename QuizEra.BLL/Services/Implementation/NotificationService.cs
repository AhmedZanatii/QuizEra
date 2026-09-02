using Microsoft.Extensions.Logging;
using QuizEra.BLL.ModelVM.Notification;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace QuizEra.BLL.Services.Implementation
{
    public class NotificationService : INotificationService
    {
        private readonly IGenericRepository<Notification> _notificationRepo;
        private readonly IGenericRepository<StudentCourse> _studentCourseRepo;
        private readonly IRealTimeNotificationService _realTimeService;
        public NotificationService(
            IGenericRepository<Notification> notificationRepo,
            IGenericRepository<StudentCourse> studentCourseRepo,
            IRealTimeNotificationService realTimeService)
        {
            _notificationRepo = notificationRepo;
            _studentCourseRepo = studentCourseRepo;
            _realTimeService = realTimeService;
        }

        public async Task CreateAndBroadcastExamNotificationAsync(int courseId, string courseName, string examTitle, int examId)
        {
            try
            {
                var includeStudent = new List<Expression<Func<StudentCourse, object>>>
                {
                    sc => sc.Student
                };

                var studentCourses = await _studentCourseRepo.Get(
                    filter: sc => sc.CourseId == courseId,
                    includeProperties: includeStudent,
                    noTrack: true
                );

                var notifications = studentCourses
                    .Where(sc => sc.Student != null && !string.IsNullOrEmpty(sc.Student.AppUserId))
                    .Select(sc => new Notification
                    {
                        TargetUserId = sc.Student.AppUserId,
                        CourseId = courseId,
                        ExamId = examId,
                        Title = "New Exam Posted",
                        Message = $"A new exam '{examTitle}' has been added to {courseName}.",
                        CreatedAt = DateTime.Now,
                        IsRead = false
                    })
                    .ToList();

                if (notifications.Any())
                {
                    await _notificationRepo.AddRangeAsync(notifications);
                    await _notificationRepo.SaveAsync();
                }

                await _realTimeService.SendExamNotificationToGroupAsync(courseId, courseName, examTitle, examId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in CreateAndBroadcastExamNotificationAsync: {ex.Message}");
            }
        }

        public async Task<List<NotificationVM>> GetUserNotificationsAsync(string appUserId)
        {
            var notifications = await _notificationRepo.Get(
                filter: n => n.TargetUserId == appUserId,
                orderBy: q => q.OrderByDescending(n => n.CreatedAt),
                noTrack: true
            );

            return notifications.Select(n => new NotificationVM
            {
                Id = n.Id,
                Title = n.Title,
                Message = n.Message,
                ExamId = n.ExamId,
                IsRead = n.IsRead,
                TimeAgo = n.CreatedAt.ToString("g")
            }).ToList();
        }

        public async Task<bool> MarkAllAsReadAsync(string appUserId)
        {
            var unread = await _notificationRepo.Get(
                filter: n => n.TargetUserId == appUserId && !n.IsRead
            );

            if (!unread.Any()) 
                return true;

            foreach (var item in unread)
            {
                item.IsRead = true;
                _notificationRepo.Update(item);
            }

            await _notificationRepo.SaveAsync();
            return true;
        }
    }
}