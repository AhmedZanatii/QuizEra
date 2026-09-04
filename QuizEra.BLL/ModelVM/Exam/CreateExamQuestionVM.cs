using System;
using System.Collections.Generic;
using System.Text;

using System.ComponentModel.DataAnnotations;

namespace QuizEra.BLL.ModelVM.Exam
{
    public class CreateExamQuestionVM
    {
        public int ExamQuestionId { get; set; }

        public int QuestionId { get; set; }

        public string QuestionText { get; set; } = string.Empty;

        public bool IsSelected { get; set; }
        public bool IsBonus { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Actual mark cannot be negative")]
        public double ActualMark { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Bonus mark cannot be negative")]
        public double BonusMark { get; set; }


        //public double NegativeMark { get; set; }
    }
}