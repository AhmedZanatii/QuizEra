using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuizEra.BLL.ModelVM.Course
{
    public class CourseVM
    {
        public int Id { get; set; }
        public int InstructorId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseLevel { get; set; } = string.Empty;
        public Guid CourseCode { get; set; }
        public string? CourseDescription { get; set; }
    }
}
