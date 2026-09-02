using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

namespace QuizEra.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly IInstructorService _instructorService;

        public InstructorController(IInstructorService instructorService)
        {
            _instructorService = instructorService;
        }

        public async Task<IActionResult> Index()
        {
            // Get current user ID
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                return View();

            // Fetch dashboard stats
            var dashboardStats = await _instructorService.GetInstructorDashboardStatsAsync(userId);

            return View(dashboardStats);
        }

        // Redirect placeholders so frontend links stay functional
        [HttpGet]
        public IActionResult Courses()
        {
            return RedirectToAction("InstructorCourses", "Course");
        }

        [HttpGet]
        public IActionResult Exams()
        {
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public IActionResult Students()
        {
            // Instructors don't have a global students list; fallback to home
            return RedirectToAction("Index", "Home");
        }
    }
}