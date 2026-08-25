namespace QuizEra.BLL.ModelVM.Administration
{
    public class AdminDashboardVM
    {
        public int TotalStudents { get; set; }
        public int ActiveStudents { get; set; }
        public int DeactivatedStudents { get; set; }

        public int TotalInstructors { get; set; }
        public int ActiveInstructors { get; set; }
        public int DeactivatedInstructors { get; set; }

        public int TotalCourses { get; set; }
        public int TotalTopics { get; set; }
        public int TotalQuestions { get; set; }
        public int ActiveQuestions { get; set; }
        public int DeletedQuestions { get; set; }
        public int TotalExams { get; set; }
    }
}