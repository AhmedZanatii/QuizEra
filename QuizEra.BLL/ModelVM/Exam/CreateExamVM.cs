using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.Exam
{
    //used for the data comming from the instructor
    public class CreateExamVM
    {
        [Required(ErrorMessage = "Course is required")]
        public int CourseId { get; set; }

        [Required(ErrorMessage = "Topic is required")]
        public int TopicId { get; set; }

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
