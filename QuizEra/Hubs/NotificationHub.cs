using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuizEra.Hubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly ICourseService _courseService;

        public NotificationHub(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public override async Task OnConnectedAsync()
        {
            string? appUserId = Context.User?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (!string.IsNullOrEmpty(appUserId))
            {
                var enrolledCourses = await _courseService.GetCoursesByStudentAsync(appUserId);
                foreach (var course in enrolledCourses)
                {
                    await Groups.AddToGroupAsync(Context.ConnectionId, $"Course_{course.Id}");
                }
            }

            await base.OnConnectedAsync();
        }

        public async Task JoinCourseGroup(int courseId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, $"Course_{courseId}");
        }

        public async Task LeaveCourseGroup(int courseId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, $"Course_{courseId}");
        }
    }
}