using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Questions;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionService _questionService;
        private readonly IGenericRepository<Topic> _topicRepository;

        public QuestionsController(
            IQuestionService questionService,
            IGenericRepository<Topic> topicRepository)
        {
            _questionService = questionService;
            _topicRepository = topicRepository;
        }

        private async Task LoadTopicsAsync()
        {
            var topics = await _topicRepository.Get();

            ViewBag.Topics = topics.ToList();
        }


        // =========================
        // Get All Questions
        // =========================

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var questions = await _questionService.GetAllAsync();

            return View(questions);
        }


        // =========================
        // Get Question
        // =========================

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var question = await _questionService.GetByIdAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            return View(question);
        }


        // =========================
        // Create Question
        // =========================

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadTopicsAsync();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadTopicsAsync();

                return View(vm);
            }

            var creatorUser = User.Identity?.Name ?? "System";

            await _questionService.AddAsync(vm, creatorUser);

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // Edit Question
        // =========================

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var question = await _questionService.GetByIdAsync(id);

            if (question == null)
            {
                return NotFound();
            }

            await LoadTopicsAsync();

            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                await LoadTopicsAsync();

                return View(vm);
            }

            var modifierUser = User.Identity?.Name ?? "System";

            await _questionService.UpdateAsync(vm, modifierUser);

            return RedirectToAction(nameof(Index));
        }


        // =========================
        // Delete Question
        // =========================

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var deleterUser = User.Identity?.Name ?? "System";

            await _questionService.DeleteAsync(id, deleterUser);

            return RedirectToAction(nameof(Index));
        }
    }
}