using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Course;
using QuizEra.BLL.Services;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

namespace QuizEra.PL.Controllers
{
    [Authorize]
    public class CourseController : Controller
    {
        private readonly ICourseService _courseService;
        private readonly ITopicService _topicService;

        public CourseController(
            ICourseService courseService,
            ITopicService topicService)
        {
            _courseService = courseService;
            _topicService = topicService;
        }

        #region Read Operations

        [Authorize(Roles = "Instructor")]
        [HttpGet]
        public async Task<IActionResult> InstructorCourses()
        {
            string userId = GetCurrentUserId();
            var courses = await _courseService.GetCoursesByInstructorAsync(userId);
            return View(courses);
        }

        [Authorize(Roles = "Student")]
        [HttpGet]
        public async Task<IActionResult> EnrolledCourses()
        {
            string userId = GetCurrentUserId();
            var courses = await _courseService.GetCoursesByStudentAsync(userId);
            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);

            if (course == null)
            {
                return NotFound();
            }

            var topics = await _topicService.GetTopicsByCourseAsync(id);

            var model = new CourseDetailsVM
            {
                Id = course.Id,
                CourseName = course.CourseName,
                CourseCode = course.CourseCode,
                CourseLevel = course.CourseLevel,
                Description = course.CourseDescription,
                Topics = topics
            };

            return View(model);
        }

        #endregion

        #region Create Operations

        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCourseVM());
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCourseVM model)
        {
            // Assign AppUserId (GUID string) directly to InstructorId property
            model.InstructorId = GetCurrentUserId();

            ModelState.Remove(nameof(model.InstructorId));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.CreatorUser = User.Identity?.Name ?? "SystemUser";

            bool isCreated = await _courseService.CreateCourseAsync(model);
            if (!isCreated)
            {
                ModelState.AddModelError("", "Instructor profile not found or an error occurred while creating the course.");
                return View(model);
            }

            return RedirectToAction(nameof(InstructorCourses));
        }

        #endregion

        #region Edit Operations

        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null)
            {
                return NotFound();
            }

            var updateVM = new UpdateCourseVM
            {
                CourseName = course.CourseName,
                CourseLevel = course.CourseLevel,
                CourseDescription = course.CourseDescription
            };

            return View(updateVM);
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public async Task<IActionResult> Edit(int id, UpdateCourseVM model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            var existingCourse = await _courseService.GetCourseByIdAsync(id);
            if (existingCourse == null)
            {
                return NotFound();
            }

            model.InstructorId = GetCurrentUserId();

            ModelState.Remove(nameof(model.InstructorId));

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            model.ModifierUser = User.Identity?.Name ?? "SystemUser";

            bool isUpdated = await _courseService.UpdateCourseAsync(model);
            if (!isUpdated)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        #endregion

        #region Delete Operations

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            string deleterUser = User.Identity?.Name ?? "SystemUser";

            bool isDeleted = await _courseService.DeleteCourseAsync(id, deleterUser);
            if (!isDeleted)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(InstructorCourses));
        }

        #endregion

        #region Helper Method

        private string GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }

            return userId;
        }

        #endregion
    }
}