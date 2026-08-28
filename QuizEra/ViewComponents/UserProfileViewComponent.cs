using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizEra.DAL.Entities;
using System.Security.Claims;

namespace QuizEra.ViewComponents
{
    public class UserProfileViewComponent : ViewComponent
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public UserProfileViewComponent(
            UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            if (!User.Identity?.IsAuthenticated ?? true)
            {
                return Content(string.Empty);
            }

            var userId =
                UserClaimsPrincipal.FindFirstValue(
                    ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Content(string.Empty);
            }

            var user =
                await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Content(string.Empty);
            }

            var roles =
                await _userManager.GetRolesAsync(user);

            ViewBag.Role =
                roles.FirstOrDefault() ?? "";

            return View(user);
        }
    }
}