namespace QuizEra.DAL.Entities
{
    public class QuestionOption
    {
        public int Id { get; private set; }

        public int QuestionId { get; private set; }

        public string OptionText { get; private set; }

        public bool IsCorrect { get; private set; }

        // Navigation Property
        public Question Question { get; private set; }

        protected QuestionOption() { }

        public QuestionOption( int questionId,string optionText, bool isCorrect)
        {
            QuestionId = questionId;
            OptionText = optionText;
            IsCorrect = isCorrect;
        }

        public void Update( string optionText, bool isCorrect)
        {
            OptionText = optionText;
            IsCorrect = isCorrect;
        }
    }
}