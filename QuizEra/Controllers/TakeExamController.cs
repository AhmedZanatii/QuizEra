using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Questions;
using QuizEra.BLL.ModelVM.StudentExamQuestionAnswer;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.Web.ViewModels;
using System.Security.Claims;

namespace QuizEra.Web.Controllers
{
    [Authorize(Roles = "Student")]
    public class TakeExamController : Controller
    {
        private readonly IStudentExamAttemptService _attemptService;
        private readonly IExamService _examService;
        private readonly IQuestionService _questionService;
        private readonly IExamAttemptResultService _resultService;
        private readonly IExamGradingService _gradingService;

        public TakeExamController(
            IStudentExamAttemptService attemptService, 
            IExamService examService,
            IQuestionService questionService,
            IExamAttemptResultService resultService,
            IExamGradingService gradingService)
        {
            _attemptService = attemptService;
            _examService = examService;
            _questionService = questionService;
            _resultService = resultService;
            _gradingService = gradingService;
        }

        [HttpGet]
        public async Task<IActionResult> ExamRules(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null) return NotFound("Exam not found.");

            return View(exam);
        }

        [HttpGet]
        public async Task<IActionResult> StartExam(int id)
        {
            var exam = await _examService.GetExamByIdAsync(id);
            if (exam == null) return NotFound("Exam not found.");

            var userId = GetCurrentUserId();

            var attempt = await _attemptService.StartAttemptAsync(id, userId, userId);

            if (attempt.EndTime is not null)
                return RedirectToAction(nameof(Result), new { id });

            var elapsedSeconds = Math.Max(0, (int)(DateTime.UtcNow - attempt.StartTime).TotalSeconds);
            if (elapsedSeconds >= exam.Duration * 60)
            {
                await _attemptService.CompleteAttemptAsync(id, userId, userId);
                return RedirectToAction(nameof(Result), new { id });
            }

            var remainingTimeSeconds = Math.Max(0, exam.Duration * 60 - elapsedSeconds);

            var uiModel = new ExamTakeUIViewModel
            {
                ExamId = exam.Id,
                StudentId = userId,
                ExamTitle = exam.Title,
                AttemptId = attempt.AttemptId,
                RemainingTimeSeconds = remainingTimeSeconds,
                Questions = new List<QuestionUIModel>()
            };

            var questionOrder = BuildShuffledOrder(attempt.ShuffleSeed, exam.Questions.Count);

            foreach (var questionIndex in questionOrder)
            {
                var examQuestion = exam.Questions[questionIndex];
                var question = await _questionService.GetByIdAsync(examQuestion.QuestionId);
                if (question == null)
                    continue;

                // 1. Handle Shuffling based on QuestionFormat
                List<QuestionOptionVM> orderedOptions = question.Options.ToList();

                if (question.QuestionFormat == QuizEra.DAL.Entities.Enums.QuestionFormat.MCQ)
                {
                    // Only shuffle Multiple Choice (MSQ)
                    var optionOrder = BuildShuffledOrder(
                        attempt.ShuffleSeed ^ (examQuestion.ExamQuestionId * 397) ^ (question.Id * 131),
                        question.Options.Count);

                    orderedOptions = optionOrder
                        .Select(optionIndex => question.Options[optionIndex])
                        .ToList();
                }
                else if (question.QuestionFormat == QuizEra.DAL.Entities.Enums.QuestionFormat.TrueFalse)
                {
                    // For True/False, we want to ensure consistent order
                    orderedOptions = question.Options.OrderByDescending(o => o.OptionText).ToList();
                }
                // For Essay, orderedOptions remains empty.

                uiModel.Questions.Add(new QuestionUIModel
                {
                    QuestionId = question.Id,
                    ExamQuestionId = examQuestion.ExamQuestionId,
                    QuestionNumber = uiModel.Questions.Count + 1,
                    Text = question.QuestionText,
                    QuestionFormat = question.QuestionFormat.ToString(), // Pass format to JS
                    SelectedAnswer = attempt?.StudentExamQuestionAnswers
                        .FirstOrDefault(a => a.ExamQuestionId == examQuestion.ExamQuestionId)?.QuestionAnswer ?? string.Empty,
                    Options = orderedOptions.Select((option, index) => new OptionUIModel
                    {
                        Label = ((char)('A' + index)).ToString(),
                        Text = option.OptionText,
                        Value = option.OptionText
                    }).ToList()
                });
            }

            return View(uiModel);
        }

        [HttpPost]
        public async Task<IActionResult> SubmitExam(int examId)
        {
            var currentUser = GetCurrentUserId();
            var attempt = await _attemptService.GetExactAttemptAsync(examId, currentUser);
            await _gradingService.GradeAttemptAsync(attempt.AttemptId, currentUser);
            await _attemptService.CompleteAttemptAsync(examId, currentUser, currentUser);

            return RedirectToAction(nameof(Result), new { examId });
        }

        [HttpGet]
        public async Task<IActionResult> Result(int examId)
        {
            var attempt = await _attemptService.GetExactAttemptAsync(examId, GetCurrentUserId());
            var result = await _resultService.GetAttemptResultAsync(attempt.AttemptId);
            return View(result);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAnswer([FromBody] SaveAnswerRequest request)
        {
            if (request == null || request.ExamId <= 0 || request.ExamQuestionId <= 0 || string.IsNullOrWhiteSpace(request.Answer))
                return BadRequest();

            var currentUser = GetCurrentUserId();
            var attempt = await _attemptService.GetExactAttemptAsync(request.ExamId, currentUser);
            if (request.AttemptId <= 0 || request.AttemptId != attempt.AttemptId)
                return BadRequest("A valid attempt is required.");

            await _attemptService.AddAnswerAsync(attempt, new StudentExamQuestionAnswerVM
            {
                StudentExamAttemptId = attempt.AttemptId,
                ExamQuestionId = request.ExamQuestionId,
                QuestionAnswer = request.Answer,
                TimeSpent = TimeSpan.FromSeconds(Math.Max(0, request.TimeSpentSeconds))
            }, currentUser);

            return Ok();
        }

        private static List<int> BuildShuffledOrder(int seed, int count)
        {
            if (count <= 1)
                return Enumerable.Range(0, count).ToList();

            var order = Enumerable.Range(0, count).ToList();
            var random = new Random(seed);

            for (int i = order.Count - 1; i > 0; i--)
            {
                var j = random.Next(0, i + 1);
                (order[i], order[j]) = (order[j], order[i]);
            }

            return order;
        }

        private string GetCurrentUserId()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
            }

            return userId;
        }
    }

    // Helper DTO for AJAX request
    public class SaveAnswerRequest
    {
        public int ExamId { get; set; }
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int ExamQuestionId { get; set; }
        public string? Answer { get; set; }
        public int TimeSpentSeconds { get; set; }
    }
}