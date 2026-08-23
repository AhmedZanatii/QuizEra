using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.Services.Abstraction;
using System.Threading.Tasks;

namespace QuizEra.PL.Controllers
{
    [Authorize]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public AnalyticsController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }

        // View for a Student to see their own result
        [Authorize(Roles = "Student, Instructor, Admin")]
        [HttpGet]
        public async Task<IActionResult> StudentResult(int attemptId)
        {
            var model = await _analyticsService.GetStudentAnalyticsAsync(attemptId);
            if (model == null) return View("StudentAnalytics", null);

            // Tell it exactly which view file to use
            return View("StudentAnalytics", model);
        }

        // Dashboard for an Instructor to see how the whole class performed on an exam
        // Dashboard for an Instructor to see how the whole class performed on an exam
        [Authorize(Roles = "Instructor, Admin")]
        [HttpGet]
        public async Task<IActionResult> ClassAnalytics(int? examId)
        {
            
            if (examId == null || examId == 0)
            {
                return View("ClassAnalytics", null);
            }

            var model = await _analyticsService.GetClassAnalyticsAsync(examId.Value);

        
            if (model == null || string.IsNullOrEmpty(model.ExamTitle))
            {
                return View("ClassAnalytics", null);
            }

            return View("ClassAnalytics", model);
        }
    }
}