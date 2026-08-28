using QuizEra.BLL.ModelVM.Administration;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
    public class AdminService : IAdminService
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Instructor> _instructorRepository;
        private readonly IGenericRepository<Course> _courseRepository;
        private readonly IGenericRepository<Topic> _topicRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Exam> _examRepository;

        public AdminService(
            IGenericRepository<Student> studentRepository,
            IGenericRepository<Instructor> instructorRepository,
            IGenericRepository<Course> courseRepository,
            IGenericRepository<Topic> topicRepository,
            IGenericRepository<Question> questionRepository,
            IGenericRepository<Exam> examRepository)
        {
            _studentRepository = studentRepository;
            _instructorRepository = instructorRepository;
            _courseRepository = courseRepository;
            _topicRepository = topicRepository;
            _questionRepository = questionRepository;
            _examRepository = examRepository;
        }

        public async Task<AdminDashboardVM> GetDashboardAsync()
        {
            var students = await _studentRepository.Get(
                includeProperties: new List<
                    System.Linq.Expressions.Expression<
                        Func<Student, object>>>
                {
            s => s.AppUser
                });

            var instructors = await _instructorRepository.Get(
                includeProperties: new List<
                    System.Linq.Expressions.Expression<
                        Func<Instructor, object>>>
                {
            i => i.AppUser
                });

            var courses = await _courseRepository.Get();
            var topics = await _topicRepository.Get();
            var questions = await _questionRepository.Get();
            var exams = await _examRepository.Get();

            return new AdminDashboardVM
            {
                // ================= STUDENTS =================

                TotalStudents = students.Count(),

                ActiveStudents = students.Count(
                    s => s.AppUser != null &&
                         s.AppUser.IsActive),

                DeactivatedStudents = students.Count(
                    s => s.AppUser != null &&
                         !s.AppUser.IsActive),


                // ================= INSTRUCTORS =================

                TotalInstructors = instructors.Count(),

                ActiveInstructors = instructors.Count(
                    i => i.AppUser != null &&
                         i.AppUser.IsActive),

                DeactivatedInstructors = instructors.Count(
                    i => i.AppUser != null &&
                         !i.AppUser.IsActive),


                // ================= COURSES =================

                TotalCourses = courses.Count(),

                ActiveCourses = courses.Count(
                    c => !c.IsDeleted),

                DeletedCourses = courses.Count(
                    c => c.IsDeleted),


                // ================= TOPICS =================

                TotalTopics = topics.Count(),
                ActiveTopics = topics.Count(t => !t.IsDeleted),
                DeletedTopics = topics.Count(t => t.IsDeleted),


                // ================= QUESTIONS =================

                TotalQuestions = questions.Count(),

                ActiveQuestions = questions.Count(
                    q => !q.IsDeleted),

                DeletedQuestions = questions.Count(
                    q => q.IsDeleted),


                // ================= EXAMS =================

                TotalExams = exams.Count(),
                // Exams are not tracked as active/deleted yet; default to 0 until implemented
                ActiveExams = 0,
                DeletedExams = 0
            };
        }
    }
}