namespace QuizEra.DAL.Entities
{
    public class StudentExamAttempt : BaseEntity
    {
        public int Id { get; private set; }

        public int ExamId { get; private set; }
        public int StudentId { get; private set; }

        public int StudResult { get; private set; }
        public int ShuffleSeed { get; private set; }

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
            DateTime startTime,
            string CreatorUser,
            int? shuffleSeed = null) : base(CreatorUser)
        {
            ExamId = examId;
            StudentId = studentId;
            StudResult = studResult;
            StartTime = startTime;
            ShuffleSeed = shuffleSeed ?? new Random().Next(1, int.MaxValue);
        }

        public void SetShuffleSeed(int shuffleSeed)
        {
            ShuffleSeed = shuffleSeed;
        }

        public void EndAttempt(DateTime endTime)
        {
            EndTime = endTime;
        }

        public void UpdateResult(int studResult, string ModifierUser)
        {
            StudResult = studResult;
            base.Update(ModifierUser);
        }

        public void AnswerQuestion(StudentExamQuestionAnswer answer, string user)
        {
            // Find if this question was already answered in this attempt
            var existingAnswer = StudentExamQuestionAnswers
                .FirstOrDefault(a => a.ExamQuestionsId == answer.ExamQuestionsId);

            if (existingAnswer != null)
            {
                existingAnswer.Update(
                    answer.StudQMarks,
                    answer.QuestionAnswer,
                    user,
                    answer.IsCorrect,
                    answer.TimeSpent,
                    answer.AIJustification);
            }
            else
            {
                StudentExamQuestionAnswers.Add(new StudentExamQuestionAnswer(
                    answer.ExamQuestionsId,
                    answer.StudentExamAttemptId,
                    answer.StudQMarks,
                    answer.QuestionAnswer,
                    user,
                    answer.IsCorrect,
                    answer.TimeSpent,
                    answer.AIJustification));
            }
        }
    }
}