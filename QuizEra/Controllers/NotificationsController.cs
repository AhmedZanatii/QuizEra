using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuizEra.PL.Controllers
{
    [Authorize]
    public class NotificationsController : Controller
    {
        private readonly INotificationService _notificationService;

        public NotificationsController(INotificationService notificationService)
        {
            _notificationService = notificationService;
        }

        
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            string? appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) return RedirectToAction("Login", "Account");

            var notifications = await _notificationService.GetUserNotificationsAsync(appUserId);
            return View(notifications);
        }

        
        [HttpGet]
        [Route("api/notifications")]
        public async Task<IActionResult> GetNotificationsApi()
        {
            string? appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) return Unauthorized();

            var notifications = await _notificationService.GetUserNotificationsAsync(appUserId);
            return Ok(notifications);
        }

        
        [HttpPost]
        [Route("api/notifications/mark-as-read")]
        public async Task<IActionResult> MarkAsReadApi()
        {
            string? appUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(appUserId)) return Unauthorized();

            bool result = await _notificationService.MarkAllAsReadAsync(appUserId);
            return Ok(new { success = result });
        }
    }
}