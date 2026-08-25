using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace QuizEra.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
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
        public IActionResult Exams()
        {
            return RedirectToAction("Index", "Home");

        }

        [HttpGet]
        public IActionResult Results()
        {
            // Fallback to home or analytics list in future
            return RedirectToAction("Index", "Home");
        }
    }
}