using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Questions;
using QuizEra.BLL.Services.Abstraction;

namespace QuizEra.Controllers
{
    public class QuestionsController : Controller
    {
        private readonly IQuestionService _questionService;

        public QuestionsController(IQuestionService questionService)
        {
            _questionService = questionService;
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
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _questionService.AddAsync(vm);

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

            return View(question);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(QuestionVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _questionService.UpdateAsync(vm);

            return RedirectToAction(nameof(Index));
        }

        // =========================
        // Delete Question
        // =========================

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _questionService.DeleteAsync(id);

            return RedirectToAction(nameof(Index));
        }
    }
}