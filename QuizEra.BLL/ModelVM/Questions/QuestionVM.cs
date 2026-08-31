using QuizEra.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuizEra.BLL.ModelVM.Questions
{
    public class QuestionVM
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Topic ID is required")]
        public int TopicID { get; set; }
        public string TopicName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        [Required(ErrorMessage = "Question text is required")]
        [MinLength(1, ErrorMessage = "Question text must contain at least 1 character")]
        public string QuestionText { get; set; } = string.Empty;

       
        [Required(ErrorMessage = "Question format is required")]
        public QuestionFormat QuestionFormat { get; set; }

        public string? QuestionAnswer { get; set; }

        [Required(ErrorMessage = "Difficulty level is required")]
        public DifficultyLevel DifficultyLevel { get; set; }

        public string? Photo { get; set; }

        public List<QuestionOptionVM> Options { get; set; }
    = new List<QuestionOptionVM>();

        // Selected correct option index
        public int? CorrectOption { get; set; }
        public object? QuestionType { get; set; }
        public bool IsDeleted { get; set; }
    }
}