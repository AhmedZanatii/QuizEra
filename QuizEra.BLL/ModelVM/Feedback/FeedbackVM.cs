using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.Feedback
{
    public class FeedbackVM
    {
        public int Id { get; set; }
        public string StudentID { get; set; }
        public int CourseID { get; set; }
        public string Comment { get; set; }
        [Required]
        [Range(0, 5, ErrorMessage = "Rate has to be between 0 and 5 :(")]
        public int Rate { get; set; }
    }
}
