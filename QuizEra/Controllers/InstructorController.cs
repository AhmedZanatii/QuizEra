using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuizEra.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        public IActionResult Index()
        {
            return View();
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