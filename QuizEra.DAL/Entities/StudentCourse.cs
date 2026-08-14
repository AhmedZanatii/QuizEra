namespace QuizEra.DAL.Entities
{
    public class StudentCourse
    {
        public int StudentId { get; private set; }
        public int CourseId { get; private set; }

        // Navigation Properties
        public Student Student { get; private set; }
        public Course Course { get; private set; }

        protected StudentCourse() { }

        public StudentCourse(int studentId, int courseId)
        {
            StudentId = studentId;
            CourseId = courseId;
        }
    }
}