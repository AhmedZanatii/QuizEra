using QuizEra.DAL.Entities.Enums;

namespace QuizEra.BLL.ModelVM.Question
{
    public class QuestionVM
    {
        public int Id { get; set; }
        public int TopicId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string QuestionType { get; set; } = string.Empty;
        public string Difficulty { get; set; } = string.Empty;
    }
}
