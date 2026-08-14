namespace QuizEra.DAL.Entities
{
    public class StudentExamAttempt
    {
        public int Id { get; private set; }

        public int ExamId { get; private set; }
        public int StudentId { get; private set; }

        public int StudResult { get; private set; }

        // Navigation Properties
        public Exam Exam { get; private set; }
        public Student Student { get; private set; }

        public ICollection<StudentExamQuestionAnswer> StudentExamQuestionAnswers
        { get; private set; } = new List<StudentExamQuestionAnswer>();

        protected StudentExamAttempt() { }

        public StudentExamAttempt(
            int examId,
            int studentId,
            int studResult)
        {
            ExamId = examId;
            StudentId = studentId;
            StudResult = studResult;
        }

        public void Update(int studResult)
        {
            StudResult = studResult;
        }
    }
}