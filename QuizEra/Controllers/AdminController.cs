using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Administration;
using QuizEra.BLL.ModelVM.Course;
using QuizEra.BLL.ModelVM.Questions;
using QuizEra.BLL.ModelVM.Topic;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.BLL.Services.Implementation;

namespace QuizEra.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;
        private readonly IStudentService _studentService;
        private readonly IInstructorService _instructorService;
        private readonly ICourseService _courseService;
        private readonly ITopicService _topicService;
        private readonly IQuestionService _questionService;
        private readonly IExamService _examService;
        public AdminController(
            IAdminService adminService,
            IStudentService studentService,
            IInstructorService instructorService,
            ICourseService courseService,
            ITopicService topicService,
            IQuestionService questionService, IExamService examService)
        {
            _adminService = adminService;
            _studentService = studentService;
            _instructorService = instructorService;
            _courseService = courseService;
            _topicService = topicService;
            _questionService = questionService;
            _examService = examService;
        }

        // =========================
        // Admin Dashboard
        // =========================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var dashboard = await _adminService.GetDashboardAsync();

            return View(dashboard);
        }


        // =====================================================
        // STUDENTS
        // =====================================================

        // =========================
        // Students List
        // =========================

        [HttpGet]
        public async Task<IActionResult> Students()
        {
            var students = await _studentService.GetAllAsync();

            return View(students);
        }


        // =========================
        // Student Details
        // =========================

        [HttpGet]
        public async Task<IActionResult> StudentDetails(int id)
        {
            var student = await _studentService.GetByIdAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }


        // =========================
        // Create Student - GET
        // =========================

        [HttpGet]
        public IActionResult CreateStudent()
        {
            return View();
        }


        // =========================
        // Create Student - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStudent(
            StudentModelVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result =
                await _studentService.CreateAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create student. Email may already exist.");

                return View(vm);
            }

            TempData["Success"] =
                "Student created successfully.";

            return RedirectToAction(nameof(Students));
        }


        // =========================
        // Edit Student - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> EditStudent(int id)
        {
            var student =
                await _studentService.GetByIdAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }


        // =========================
        // Edit Student - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditStudent(
            StudentModelVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result =
                await _studentService.UpdateAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update student. Email may already exist.");

                return View(vm);
            }

            TempData["Success"] =
                "Student updated successfully.";

            return RedirectToAction(nameof(Students));
        }


        // =========================
        // Deactivate Student - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> DeactivateStudent(int id)
        {
            var student =
                await _studentService.GetByIdAsync(id);

            if (student == null)
                return NotFound();

            return View(student);
        }


        // =========================
        // Deactivate Student - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeactivateStudentConfirmed(int id)
        {
            var result =
                await _studentService.DeactivateAsync(id);

            if (!result)
            {
                TempData["Error"] =
                    "Unable to deactivate student.";

                return RedirectToAction(nameof(Students));
            }

            TempData["Success"] =
                "Student deactivated successfully.";

            return RedirectToAction(nameof(Students));
        }


        // =========================
        // Restore Student
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreStudent(int id)
        {
            var result =
                await _studentService.RestoreAsync(id);

            if (!result)
            {
                TempData["Error"] =
                    "Unable to restore student.";

                return RedirectToAction(nameof(Students));
            }

            TempData["Success"] =
                "Student restored successfully.";

            return RedirectToAction(nameof(Students));
        }


        // =====================================================
        // INSTRUCTORS
        // =====================================================

        // =========================
        // Instructors List
        // =========================

        [HttpGet]
        public async Task<IActionResult> Instructors()
        {
            var instructors =
                await _instructorService.GetAllAsync();

            return View(instructors);
        }


        // =========================
        // Instructor Details
        // =========================

        [HttpGet]
        public async Task<IActionResult> InstructorDetails(int id)
        {
            var instructor =
                await _instructorService.GetByIdAsync(id);

            if (instructor == null)
                return NotFound();

            return View(instructor);
        }


        // =========================
        // Create Instructor - GET
        // =========================

        [HttpGet]
        public IActionResult CreateInstructor()
        {
            return View();
        }


        // =========================
        // Create Instructor - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateInstructor(
            InstructorModelVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result =
                await _instructorService.CreateAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create instructor. Email may already exist.");

                return View(vm);
            }

            TempData["Success"] =
                "Instructor created successfully.";

            return RedirectToAction(nameof(Instructors));
        }


        // =========================
        // Edit Instructor - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> EditInstructor(int id)
        {
            var instructor =
                await _instructorService.GetByIdAsync(id);

            if (instructor == null)
                return NotFound();

            return View(instructor);
        }


        // =========================
        // Edit Instructor - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInstructor(
            InstructorModelVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result =
                await _instructorService.UpdateAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update instructor. Email may already exist.");

                return View(vm);
            }

            TempData["Success"] =
                "Instructor updated successfully.";

            return RedirectToAction(nameof(Instructors));
        }


        // =========================
        // Deactivate Instructor - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> DeactivateInstructor(int id)
        {
            var instructor =
                await _instructorService.GetByIdAsync(id);

            if (instructor == null)
                return NotFound();

            return View(instructor);
        }


        // =========================
        // Deactivate Instructor - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult>
            DeactivateInstructorConfirmed(int id)
        {
            var result =
                await _instructorService.DeactivateAsync(id);

            if (!result)
            {
                TempData["Error"] =
                    "Unable to deactivate instructor.";

                return RedirectToAction(nameof(Instructors));
            }

            TempData["Success"] =
                "Instructor deactivated successfully.";

            return RedirectToAction(nameof(Instructors));
        }


        // =========================
        // Restore Instructor
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreInstructor(int id)
        {
            var result =
                await _instructorService.RestoreAsync(id);

            if (!result)
            {
                TempData["Error"] =
                    "Unable to restore instructor.";

                return RedirectToAction(nameof(Instructors));
            }

            TempData["Success"] =
                "Instructor restored successfully.";

            return RedirectToAction(nameof(Instructors));
        }

        // =====================================================
        // COURSES
        // =====================================================

        // =========================
        // Courses List
        // =========================

        [HttpGet]
        public async Task<IActionResult> Courses()
        {
            var courses = await _courseService
                .GetAllCoursesIncludingDeletedAsync();

            return View(courses);
        }

        [HttpGet]
        public async Task<IActionResult> CourseDetails(int id)
        {
            var course = await _courseService.GetCourseByIdIncludingDeletedAsync(id);

            if (course == null)
                return NotFound();

            return View("CourseDetails", course);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreCourse(int id)
        {
            var user = User.Identity?.Name ?? "System";

            var result = await _courseService
                .RestoreCourseAsync(id, user);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Courses));
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var user = User.Identity?.Name ?? "System";

            var result = await _courseService
                .DeleteCourseAsync(id, user);

            if (!result)
                return NotFound();

            return RedirectToAction(nameof(Courses));
        }
        // =========================
        // Create Course - GET
        // =========================
        [HttpGet]
        public async Task<IActionResult> CreateCourse()
        {
            var instructors = await _instructorService.GetAllAsync();

            ViewBag.Instructors = instructors;

            return View(new CreateCourseVM());
        }


        // =========================
        // Create Course - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(CreateCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Instructors = await _instructorService.GetAllAsync();
                return View(vm);
            }

            var user = User.Identity?.Name ?? "System";

            vm.CreatorUser = user;

            var result = await _courseService.CreateCourseAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create course. Please make sure the selected instructor exists.");

                ViewBag.Instructors = await _instructorService.GetAllAsync();

                return View(vm);
            }

            TempData["Success"] = "Course created successfully.";

            return RedirectToAction(nameof(Courses));
        }

        [HttpGet]
        public async Task<IActionResult> EditCourse(int id)
        {
            var course = await _courseService.GetCourseByIdIncludingDeletedAsync(id);

            if (course == null || course.IsDeleted)
                return NotFound();

            var instructors = await _instructorService.GetAllAsync();

            ViewBag.Instructors = instructors;

            var vm = new UpdateCourseVM
            {
                Id = course.Id,
                InstructorId = course.InstructorId,
                CourseName = course.CourseName,
                CourseLevel = course.CourseLevel,
                CourseDescription = course.CourseDescription
            };

            return View(vm);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCourse(UpdateCourseVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Instructors = await _instructorService.GetAllAsync();
                return View(vm);
            }

            var user = User.Identity?.Name ?? "System";

            vm.ModifierUser = user;

            var result = await _courseService.UpdateCourseAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update course.");

                ViewBag.Instructors = await _instructorService.GetAllAsync();

                return View(vm);
            }

            TempData["Success"] = "Course updated successfully.";

            return RedirectToAction(nameof(Courses));
        }

        // =====================================================
        // TOPICS
        // =====================================================

        // =========================
        // Topics List
        // =========================

        [HttpGet]
        public async Task<IActionResult> Topics()
        {
            var topics = await _topicService
                .GetAllTopicsIncludingDeletedAsync();

            return View(topics);
        }


        // =========================
        // Topic Details
        // =========================

        [HttpGet]
        public async Task<IActionResult> TopicDetails(int id)
        {
            var topic = await _topicService
                .GetTopicDetailsAsync(id);

            if (topic == null)
                return NotFound();

            return View(topic);
        }


        // =========================
        // Create Topic - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> CreateTopic()
        {
            var courses = await _courseService
                .GetAllCoursesIncludingDeletedAsync();

            ViewBag.Courses = courses
                .Where(c => !c.IsDeleted);

            return View(new CreateTopicVM());
        }


        // =========================
        // Create Topic - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTopic(CreateTopicVM vm)
        {
            if (!ModelState.IsValid)
            {
                var courses = await _courseService
                    .GetAllCoursesIncludingDeletedAsync();

                ViewBag.Courses = courses
                    .Where(c => !c.IsDeleted);

                return View(vm);
            }

            var user = User.Identity?.Name ?? "System";

            vm.CreatorUser = user;

            var result = await _topicService
                .CreateTopicAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to create topic.");

                var courses = await _courseService
                    .GetAllCoursesIncludingDeletedAsync();

                ViewBag.Courses = courses
                    .Where(c => !c.IsDeleted);

                return View(vm);
            }

            TempData["Success"] =
                "Topic created successfully.";

            return RedirectToAction(nameof(Topics));
        }


        // =========================
        // Edit Topic - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> EditTopic(int id)
        {
            var topic = await _topicService
                .GetTopicByIdAsync(id);

            if (topic == null)
                return NotFound();

            var courses = await _courseService
                .GetAllCoursesIncludingDeletedAsync();

            ViewBag.Courses = courses
                .Where(c => !c.IsDeleted);

            var vm = new UpdateTopicVM
            {
                Id = topic.Id,
                CourseId = topic.CourseId,
                Name = topic.Name
            };

            return View(vm);
        }


        // =========================
        // Edit Topic - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTopic(UpdateTopicVM vm)
        {
            if (!ModelState.IsValid)
            {
                var courses = await _courseService
                    .GetAllCoursesIncludingDeletedAsync();

                ViewBag.Courses = courses
                    .Where(c => !c.IsDeleted);

                return View(vm);
            }

            var user = User.Identity?.Name ?? "System";

            vm.ModifierUser = user;

            var result = await _topicService
                .UpdateTopicAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Unable to update topic.");

                var courses = await _courseService
                    .GetAllCoursesIncludingDeletedAsync();

                ViewBag.Courses = courses
                    .Where(c => !c.IsDeleted);

                return View(vm);
            }

            TempData["Success"] =
                "Topic updated successfully.";

            return RedirectToAction(nameof(Topics));
        }


        // =========================
        // Delete Topic - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            var topic = await _topicService
                .GetTopicByIdAsync(id);

            if (topic == null)
                return NotFound();

            return View(topic);
        }


        // =========================
        // Delete Topic - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTopicConfirmed(int id)
        {
            var user = User.Identity?.Name ?? "System";

            var result = await _topicService
                .DeleteTopicAsync(id, user);

            if (!result)
            {
                TempData["Error"] =
                    "Unable to delete topic.";

                return RedirectToAction(nameof(Topics));
            }

            TempData["Success"] =
                "Topic deleted successfully.";

            return RedirectToAction(nameof(Topics));
        }


        // =========================
        // Restore Topic
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreTopic(int id)
        {
            var user = User.Identity?.Name ?? "System";

            var result = await _topicService
                .RestoreTopicAsync(id, user);

            if (!result)
            {
                TempData["Error"] =
                    "Unable to restore topic.";

                return RedirectToAction(nameof(Topics));
            }

            TempData["Success"] =
                "Topic restored successfully.";

            return RedirectToAction(nameof(Topics));
        }

        // =====================================================
        // QUESTIONS
        // =====================================================
        [HttpGet]
        public async Task<IActionResult> GetTopicsByCourse(int courseId)
        {
            var topics = await _topicService.GetTopicsByCourseAsync(courseId);

            return Json(topics.Select(t => new
            {
                id = t.Id,
                name = t.Name
            }));
        }
        // =========================
        // Questions List
        // =========================

        [HttpGet]
        public async Task<IActionResult> Questions()
        {
            var questions = await _questionService
                .GetByIdAsyncIncludingDeleted();

            return View(questions);
        }


        // =========================
        // Question Details
        // =========================

        [HttpGet]
        public async Task<IActionResult> QuestionDetails(int id)
        {
            var question = await _questionService
                .GetByIdAsync(id);

            if (question == null)
                return NotFound();

            return View(question);
        }


        // =========================
        // Create Question - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> CreateQuestion()
        {
            ViewBag.Courses = await _courseService.GetAllCoursesAsync();

            ViewBag.Topics = await _topicService.GetAllTopicsAsync();

            return View(new QuestionVM());
        }

        // =========================
        // Create Question - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestion(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Courses = await _courseService.GetAllCoursesAsync();
                ViewBag.Topics = await _topicService.GetAllTopicsAsync();

                return View(vm);
            }

            var user = User.Identity?.Name ?? "System";

            await _questionService.AddAsync(vm, user);

            TempData["Success"] = "Question created successfully.";

            return RedirectToAction(nameof(Questions));
        }

        // =========================
        // Edit Question - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> EditQuestion(int id)
        {
            var question = await _questionService
                .GetByIdAsync(id);

            if (question == null)
                return NotFound();

            return View(question);
        }


        // =========================
        // Edit Question - POST
        // ======2===================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditQuestion(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Topics = await _topicService
                    .GetAllTopicsAsync();

                return View(vm);
            }

            var user = User.Identity?.Name ?? "System";

            await _questionService.UpdateAsync(vm, user);

            TempData["Success"] = "Question updated successfully.";

            return RedirectToAction(nameof(Questions));
        }


        // =========================
        // Delete / Deactivate Question - GET
        // =========================

        [HttpGet]
        public async Task<IActionResult> DeleteQuestion(int id)
        {
            var question = await _questionService
                .GetByIdAsync(id);

            if (question == null)
                return NotFound();

            return View(question);
        }


        // =========================
        // Delete / Deactivate Question - POST
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestionConfirmed(int id)
        {
            var user = User.Identity?.Name ?? "System";

            try
            {
                await _questionService.DeleteAsync(id, user);

                TempData["Success"] =
                    "Question deleted successfully.";
            }
            catch
            {
                TempData["Error"] =
                    "Unable to delete question.";
            }

            return RedirectToAction(nameof(Questions));
        }


        // =========================
        // Restore Question
        // =========================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreQuestion(int id)
        {
            try
            {
                await _questionService.RestoreAsync(id);

                TempData["Success"] =
                    "Question restored successfully.";
            }
            catch
            {
                TempData["Error"] =
                    "Unable to restore question.";
            }

            return RedirectToAction(nameof(Questions));
        }

        // =========================
        // EXAMS
        // =========================

        [HttpGet]
        public async Task<IActionResult> Exams()
        {
            var exams = await _examService.GetAllExamsAsync();

            return View(exams);
        }
        [HttpGet]
        public async Task<IActionResult> ExamDetails(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);

            if (exam == null)
                return NotFound();

            return View(exam);
        }
    }
}