using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

namespace QuizEra.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly IExamService _examService;
        private readonly IStudentExamAttemptService _attemptService;

        public StudentController(
            IExamService examService,
            IStudentExamAttemptService attemptService)
        {
            _examService = examService;
            _attemptService = attemptService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Courses()
        {
            return RedirectToAction("EnrolledCourses", "Course");
        }

        [HttpGet]
        public async Task<IActionResult> Exams()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var exams = await _examService.GetAllExamsAsync();
            var unavailableExamIds = new HashSet<int>();
            var attempts = await _attemptService.GetByStudentIdAsync(userId);

            foreach (var attempt in attempts)
            {
                var exam = exams.FirstOrDefault(item => item.Id == attempt.ExamId);
                var timerExpired = exam != null &&
                    DateTime.UtcNow >= attempt.StartTime.AddMinutes(exam.Duration);

                if (attempt.EndTime.HasValue || timerExpired)
                    unavailableExamIds.Add(attempt.ExamId);
            }

            var now = DateTime.UtcNow;
            var availableExams = exams.Where(exam =>
                exam.StartDate <= now &&
                exam.EndDate >= now &&
                !unavailableExamIds.Contains(exam.Id));

            return View("AvailableExams", availableExams);
        }

        [HttpGet]
        public async Task<IActionResult> Results()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
                return Unauthorized();

            var attempts = await _attemptService.GetByStudentIdAsync(userId);
            return View("MyResults", attempts.OrderByDescending(a => a.StartTime));
        }
    }
}