using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.ModelVM.Notification
{
    public class NotificationVM
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public int? ExamId { get; set; }
        public bool IsRead { get; set; }
        public string TimeAgo { get; set; } = string.Empty;
    }
}
