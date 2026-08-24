using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuizEra.BLL.ModelVM.Questions
{
    public class QuestionOptionVM
    {
        public int Id { get; set; }
  
        public int QuestionId { get; set; }

        [Required(ErrorMessage = "Option text is required")]
        [MinLength(1, ErrorMessage = "Option text must contain at least 1 character")]
        public string OptionText { get; set; }

        public bool IsCorrect { get; set; }
    }
}
