using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

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
                    "Unable to create the exam. Please check the exam data and selected questions.");

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
                TopicId = exam.TopicId,
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
                    "Unable to update the exam. Please check the exam data and selected questions.");

                await LoadCreateDataAsync(model);
                return View(model);
            }

            return RedirectToAction(nameof(Details), new { id = model.Id });
        }

        // =========================
        // Delete
        // =========================

        [Authorize(Roles = "Admin, Instructor")]
        [HttpPost]
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
            // Load all courses
            var courses = await _courseRepository.Get();

            ViewBag.Courses = courses.ToList();

            // If a topic has already been selected,
            // only load topics belonging to that course.
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

            // If a topic has already been selected,
            // only load questions belonging to that topic.
            if (model != null && model.TopicId > 0)
            {
                var questions = await _questionRepository.Get(
                    filter: q => q.TopicID == model.TopicId);

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
            // Load all courses
            var courses = await _courseRepository.Get();

            ViewBag.Courses = courses.ToList();

            // Load topics for selected course
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

            // Load questions for selected topic
            if (model.TopicId > 0)
            {
                var questions = await _questionRepository.Get(
                    filter: q => q.TopicID == model.TopicId);

                ViewBag.Questions = questions.ToList();
            }
            else
            {
                ViewBag.Questions = new List<Question>();
            }
        }
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
    }
}