using QuizEra.BLL.ModelVM.Question;

namespace QuizEra.BLL.ModelVM.Topic
{
    public class TopicDetailsVM
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string Name { get; set; } = string.Empty;

        public IEnumerable<QuestionVM> Questions { get; set; } = Enumerable.Empty<QuestionVM>();
    }
}
