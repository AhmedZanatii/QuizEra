using System;

namespace QuizEra.DAL.Entities
{
    public class Notification
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public string TargetUserId { get; set; } = string.Empty;
        public int? CourseId { get; set; }
        public int? ExamId { get; set; }
        public bool IsRead { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}