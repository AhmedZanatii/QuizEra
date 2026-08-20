using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Topic;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

namespace QuizEra.Controllers
{
    public class TopicController : Controller
    {
        private readonly ITopicService _topicService;
        public TopicController(ITopicService topicService)
        {
            _topicService = topicService;
        }

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

            return RedirectToAction("Details","Course",new { id = model.CourseId });
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
