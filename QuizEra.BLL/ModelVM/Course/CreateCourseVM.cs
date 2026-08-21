using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace QuizEra.BLL.ModelVM.Course
{
    public class CreateCourseVM
    {
        public string InstructorId { get; set; }

        [Required(ErrorMessage = "Course name is required.")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Course name must be between 3 and 100 characters.")]
        public string CourseName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select or enter a course level.")]
        [StringLength(50, ErrorMessage = "Course level cannot exceed 50 characters.")]
        public string CourseLevel { get; set; } = string.Empty;

        [Required(ErrorMessage = "Course description is required.")]
        [StringLength(1000, ErrorMessage = "Description cannot exceed 1000 characters.")]
        public string CourseDescription { get; set; } = string.Empty;
        public string CreatorUser { get; set; } = string.Empty;
    }
}
