using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Feedback;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

namespace QuizEra.Controllers
{
    [Authorize(Roles = "Student")]
    public class FeedbackController : Controller
    {
        private readonly IFeedbackService _feedbackService;

        public FeedbackController(IFeedbackService feedbackService)
        {
            _feedbackService = feedbackService;
        }

        [HttpGet]
        public IActionResult Create(int courseId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            return View(new FeedbackVM
            {
                CourseID = courseId,
                StudentID = userId
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Unauthorized();
            }

            model.StudentID = userId;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _feedbackService.AddAsync(model, User.Identity?.Name ?? userId);

            return RedirectToAction("CourseDetailsForStud", "Course", new { id = model.CourseID });
        }
    }
}
