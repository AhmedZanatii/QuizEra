using QuizEra.BLL.ModelVM.Topic;

namespace QuizEra.BLL.ModelVM.Course
{
    public class CourseDetailsVM
    {
        public int Id { get; set; }

        public string CourseName { get; set; }

        public Guid CourseCode { get; set; }

        public string CourseLevel { get; set; }

        public string? Description { get; set; }

        public IEnumerable<TopicVM> Topics { get; set; }
            = Enumerable.Empty<TopicVM>();
    }
}