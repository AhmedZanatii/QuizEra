using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuizEra.BLL.ModelVM.Topic
{
    public class CreateTopicVM
    {
        [Required(ErrorMessage = "Course selection is required.")]
        public int CourseId { get; set; }
        [Required(ErrorMessage = "Topic name is required.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Topic name must be between 2 and 100 characters.")]
        public string Name { get; set; } = string.Empty;
        public string CreatorUser { get; set; } = string.Empty;
    }
}
