namespace QuizEra.DAL.Entities
{
    public class StudentExamAttempt
    {
        public int Id { get; private set; }

        public int ExamId { get; private set; }
        public int StudentId { get; private set; }

        public int StudResult { get; private set; }

        public DateTime StartTime { get; private set; }
        public DateTime? EndTime { get; private set; }

        // Navigation Properties
        public Exam Exam { get; private set; }
        public Student Student { get; private set; }

        public ICollection<StudentExamQuestionAnswer> StudentExamQuestionAnswers
        { get; private set; } = new List<StudentExamQuestionAnswer>();

        protected StudentExamAttempt() { }

        public StudentExamAttempt(
            int examId,
            int studentId,
            int studResult,
            DateTime startTime)
        {
            ExamId = examId;
            StudentId = studentId;
            StudResult = studResult;
            StartTime = startTime;
        }

        public void EndAttempt(DateTime endTime)
        {
            EndTime = endTime;
        }

        public void Update(int studResult)
        {
            StudResult = studResult;
        }
    }
}