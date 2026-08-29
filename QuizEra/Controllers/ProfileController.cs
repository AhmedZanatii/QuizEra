using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Profile;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

namespace QuizEra.Controllers
{
    [Authorize]
    public class ProfileController : Controller
    {
        private readonly IProfileService _profileService;
        private readonly IWebHostEnvironment _environment;

        public ProfileController(
            IProfileService profileService,
            IWebHostEnvironment environment)
        {
            _profileService = profileService;
            _environment = environment;
        }

        // =========================
        // Profile Index
        // =========================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var profile = await _profileService.GetProfileAsync(userId);

            if (profile == null)
                return NotFound();

            // إضافة الـ Role للـ ViewBag ليتعرف عليه الـ View والـ Layout
            ViewBag.Role = User.FindFirstValue(ClaimTypes.Role) ?? profile.Role ?? "Student";

            return View(profile);
        }

        // =========================
        // Edit Profile - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            var model = await _profileService.GetEditProfileAsync(userId);

            if (model == null)
                return NotFound();

            // تمرير الـ Role للـ ViewBag
            ViewBag.Role = User.FindFirstValue(ClaimTypes.Role) ?? "Student";

            return View(model);
        }

        // =========================
        // Edit Profile - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EditProfileVM model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
                return RedirectToAction("Login", "Account");

            if (!ModelState.IsValid)
            {
                // إعادة ضبط الـ Role للـ ViewBag في حالة وجود خطأ بالإدخال لكي لا يتشوه التصميم
                ViewBag.Role = User.FindFirstValue(ClaimTypes.Role) ?? "Student";
                return View(model);
            }

            string? profileImagePath = null;

            // =========================
            // Upload Profile Image
            // =========================

            if (model.ProfileImage != null && model.ProfileImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(
                    _environment.WebRootPath,
                    "uploads",
                    "profiles");

                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var extension = Path.GetExtension(model.ProfileImage.FileName);
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfileImage.CopyToAsync(stream);
                }

                profileImagePath = $"/uploads/profiles/{fileName}";
            }

            // =========================
            // Update Profile
            // =========================

            var result = await _profileService.UpdateProfileAsync(
                userId,
                model,
                profileImagePath);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update profile.");

                ViewBag.Role = User.FindFirstValue(ClaimTypes.Role) ?? "Student";
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}