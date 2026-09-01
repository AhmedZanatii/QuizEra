using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Course;
using QuizEra.BLL.ModelVM.Exam;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.Controllers
{
    [Authorize]
    public class ExamController : Controller
    {
        private readonly IExamService _examService;
        private readonly IGenericRepository<Course> _courseRepository;
        private readonly IGenericRepository<Topic> _topicRepository;
        private readonly IGenericRepository<Question> _questionRepository;

        public ExamController(
            IExamService examService,
            IGenericRepository<Course> courseRepository,
            IGenericRepository<Topic> topicRepository,
            IGenericRepository<Question> questionRepository)
        {
            _examService = examService;
            _courseRepository = courseRepository;
            _topicRepository = topicRepository;
            _questionRepository = questionRepository;
        }

        // =========================
        // Index
        // =========================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var exams = await _examService.GetAllExamsAsync();

            return View(exams);
        }

        // =========================
        // Details
        // =========================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);

            if (exam == null)
                return NotFound();

            return View(exam);
        }

        // =========================
        // Create
        // =========================

        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadCreateDataAsync();

            return View(new CreateExamVM());
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateExamVM model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateDataAsync(model);
                return View(model);
            }

            var created = await _examService.CreateExamAsync(model);

            if (!created)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to create the exam. Please check the exam data, selected topics and selected questions.");

                await LoadCreateDataAsync(model);
                return View(model);
            }
            

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Edit
        // =========================

        [Authorize(Roles = "Admin, Instructor")]
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);

            if (exam == null)
                return NotFound();

            var model = new UpdateExamVM
            {
                Id = exam.Id,
                CourseId = exam.CourseId,
                TopicIds = exam.TopicIds,
                Title = exam.Title,
                Duration = exam.Duration,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate,
                Questions = exam.Questions
            };

            await LoadCreateDataAsync(model);

            return View(model);
        }

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UpdateExamVM model)
        {
            if (!ModelState.IsValid)
            {
                await LoadCreateDataAsync(model);
                return View(model);
            }

            var updated = await _examService.UpdateExamAsync(model);

            if (!updated)
            {
                ModelState.AddModelError(
                    "",
                    "Unable to update the exam. Please check the exam data, selected topics and selected questions.");

                await LoadCreateDataAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Delete
        // =========================

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _examService.DeleteExamAsync(id);

            if (!deleted)
                return NotFound();

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Load Create Data
        // =========================

        private async Task LoadCreateDataAsync(CreateExamVM? model = null)
        {
            // -------------------------
            // Courses
            // -------------------------

            var courses = await _courseRepository.Get();

            ViewBag.Courses = courses.ToList();

            // -------------------------
            // Topics
            // -------------------------

            if (model != null && model.CourseId > 0)
            {
                var topics = await _topicRepository.Get(
                    filter: t => t.CourseID == model.CourseId);

                ViewBag.Topics = topics.ToList();
            }
            else
            {
                ViewBag.Topics = new List<Topic>();
            }

            // -------------------------
            // Questions
            // -------------------------

            if (model != null &&
                model.TopicIds != null &&
                model.TopicIds.Any())
            {
                var questions = await _questionRepository.Get(
                    filter: q => model.TopicIds.Contains(q.TopicID));

                ViewBag.Questions = questions.ToList();
            }
            else
            {
                ViewBag.Questions = new List<Question>();
            }
        }

        // =========================
        // Load Edit Data
        // =========================

        private async Task LoadCreateDataAsync(UpdateExamVM model)
        {
            // -------------------------
            // Courses
            // -------------------------

            var courses = await _courseRepository.Get();

            ViewBag.Courses = courses.ToList();

            // -------------------------
            // Topics
            // -------------------------

            if (model.CourseId > 0)
            {
                var topics = await _topicRepository.Get(
                    filter: t => t.CourseID == model.CourseId);

                ViewBag.Topics = topics.ToList();
            }
            else
            {
                ViewBag.Topics = new List<Topic>();
            }

            // -------------------------
            // Questions
            // -------------------------

            if (model.TopicIds != null &&
                model.TopicIds.Any())
            {
                var questions = await _questionRepository.Get(
                    filter: q => model.TopicIds.Contains(q.TopicID));

                ViewBag.Questions = questions.ToList();
            }
            else
            {
                ViewBag.Questions = new List<Question>();
            }
        }

        // =========================
        // Get Topics By Course
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetTopicsByCourse(int courseId)
        {
            var topics = await _topicRepository.Get(
                filter: t => t.CourseID == courseId);

            var result = topics.Select(t => new
            {
                id = t.Id,
                name = t.Name
            });

            return Json(result);
        }

        // =========================
        // Get Questions By Topic
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetQuestionsByTopic(int topicId)
        {
            var questions = await _questionRepository.Get(
                filter: q => q.TopicID == topicId);

            var result = questions.Select(q => new
            {
                id = q.Id,
                questionText = q.QuestionText,
                questionFormat = q.QuestionFormat.ToString()
            });

            return Json(result);
        }

        // =========================
        // Get Questions By Topics
        // =========================

        [HttpGet]
        public async Task<IActionResult> GetQuestionsByTopics(string topicIds)
        {
            if (string.IsNullOrWhiteSpace(topicIds))
            {
                return Json(new List<object>());
            }

            var ids = topicIds
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(id => int.TryParse(id, out var value) ? value : 0)
                .Where(id => id > 0)
                .Distinct()
                .ToList();

            if (!ids.Any())
            {
                return Json(new List<object>());
            }

            var questions = await _questionRepository.Get(
                filter: q => ids.Contains(q.TopicID));

            var result = questions.Select(q => new
            {
                id = q.Id,
                questionText = q.QuestionText,
                questionFormat = q.QuestionFormat.ToString(),
                topicId = q.TopicID
            });

            return Json(result);
        }
    }
}