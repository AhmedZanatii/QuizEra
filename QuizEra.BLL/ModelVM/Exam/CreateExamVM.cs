using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.Exam
{
    public class CreateExamVM
    {
        [Required(ErrorMessage = "Course is required")]
        public int CourseId { get; set; }

        public List<int> TopicIds { get; set; } = new();

        [Required(ErrorMessage = "Exam title is required")]
        [MinLength(2, ErrorMessage = "Exam title must contain at least 2 characters")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required")]
        [Range(1, 200, ErrorMessage = "Duration must be between 1 and 200 minutes")]
        public int Duration { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        public DateTime EndDate { get; set; }

        public List<CreateExamQuestionVM> Questions { get; set; } = new();
    }
}