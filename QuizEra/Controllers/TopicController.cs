using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Topic;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

namespace QuizEra.PL.Controllers
{
    [Authorize]
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;

        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var model = await _topicService.GetTopicDetailsAsync(id);
            if (model == null)
                return NotFound();

            return View(model);
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var topic = await _topicService.GetTopicByIdAsync(id);
            if (topic == null)
                return NotFound();

            var model = new UpdateTopicVM
            {
                Id = topic.Id,
                CourseId = topic.CourseId,
                Name = topic.Name
            };

            return View(model);
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateTopicVM model)
        {
            if (id != model.Id)
                return BadRequest();

            var existing = await _topicService.GetTopicByIdAsync(id);
            if (existing == null)
                return NotFound();

            model.ModifierUser = User.Identity?.Name ?? "SystemUser";

            ModelState.Remove(nameof(model.ModifierUser));

            if (!ModelState.IsValid)
                return View(model);

            bool updated = await _topicService.UpdateTopicAsync(model);
            if (!updated)
                return NotFound();

            return RedirectToAction(nameof(Details), new { id });
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            // Retrieve topic to get CourseId before deletion
            var topic = await _topicService.GetTopicByIdAsync(id);
            if (topic == null)
                return NotFound();

            string deleterUser = User.Identity?.Name ?? "SystemUser";

            bool isDeleted = await _topicService.DeleteTopicAsync(id, deleterUser);
            if (!isDeleted)
                return NotFound();

            return RedirectToAction("Details", "Course", new { id = topic.CourseId });
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateTopicVM model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction(
                    "Details",
                    "Course",       
                    new { id = model.CourseId });
            }

            model.CreatorUser = GetCurrentUserId();

            await _topicService.CreateTopicAsync(model);

            return RedirectToAction("Details", "Course", new { id = model.CourseId });
        }

        private string GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }

            return userId;
        }
    }
}
