using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Feedback;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

namespace QuizEra.PL.Controllers
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
        public async Task<IActionResult> Create(int courseId)
        {
            var userId = GetCurrentUserId();
            var userFeedbacks = await _feedbackService.GetByStudentIdAsync(userId);
            var existing = userFeedbacks.FirstOrDefault(f => f.CourseID == courseId);

            if (existing != null)
                return RedirectToAction(nameof(Edit), new { id = existing.Id });

            return View(new FeedbackVM { CourseID = courseId, StudentID = userId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(FeedbackVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _feedbackService.AddAsync(model, User.Identity?.Name ?? "Student");
            return RedirectToAction("CourseDetailsForStud", "Course", new { id = model.CourseID });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var feedback = await _feedbackService.GetByIdAsync(id);
            if (feedback == null)
                return NotFound();

            return View(feedback);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(FeedbackVM model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _feedbackService.UpdateAsync(model, User.Identity?.Name ?? "Student");
            return RedirectToAction("CourseDetailsForStud", "Course", new { id = model.CourseID });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id, int courseId)
        {
            await _feedbackService.DeleteAsync(id, User.Identity?.Name ?? "Student");
            return RedirectToAction("CourseDetailsForStud", "Course", new { id = courseId });
        }

        private string GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier) ?? throw new UnauthorizedAccessException();
        }
    }
}