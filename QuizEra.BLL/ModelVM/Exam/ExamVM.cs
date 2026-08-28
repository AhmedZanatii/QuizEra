using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.ModelVM.Exam
{
    //to display an exam
    public class ExamVM
    {
        //used for the data going to the view
        public int Id { get; set; }

        public List<int> TopicIds { get; set; } = new();
        public List<string> TopicNames { get; set; } = new();
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;

        public int Duration { get; set; }

        public double TotalMarks { get; set; }

        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public List<CreateExamQuestionVM> Questions { get; set; } = new();
    }
}
