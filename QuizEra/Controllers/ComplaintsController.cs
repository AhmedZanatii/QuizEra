using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Complaint;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;
using System.Threading.Tasks;

namespace QuizEra.Web.Controllers
{
    [Authorize]
    public class ComplaintsController : Controller
    {
        private readonly IComplaintService _complaintService;
        private readonly IExamGradingService _examGradingService;

        public ComplaintsController(IComplaintService complaintService, IExamGradingService examGradingService)
        {
            _complaintService = complaintService;
            _examGradingService = examGradingService;
        }

        [Authorize(Roles = "Student")] 
        public async Task<IActionResult> StudentComplaints()
        {
            // Extract the Student ID from the logged-in user's claims
            var studentId = GetCurrentUserId();

            // Fetch complaints using your service method
            var complaints = await _complaintService.GetAllByStudentIdAsync(studentId);
            
            return View(complaints);
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Submit(ComplaintVM complaintVM) 
        {
            // Tell ASP.NET to ignore the fields the form doesn't send
            ModelState.Remove("UserStudentId");
            ModelState.Remove("ExamAttempt");
            ModelState.Remove("ExamQuestion");
            ModelState.Remove("Status");

            if (!ModelState.IsValid || complaintVM == null)
            {
                return Redirect(Request.Headers["Referer"].ToString());
            }

            complaintVM.UserStudentId = GetCurrentUserId();
            string creatorUser = User.Identity?.Name ?? "System";

            try 
            {
                await _complaintService.CreateAsync(complaintVM, creatorUser);
            } 
            catch (InvalidOperationException ex) 
            {
                return BadRequest(ex.Message);
            }

            return Redirect(Request.Headers["Referer"].ToString());
            
        }

        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> EditComment(int id, string comment)
        {
            string modifierUser = User.Identity?.Name ?? "System";
            await _complaintService.UpdateCommentAsync(id, comment, modifierUser);
            
            return RedirectToAction("StudentComplaints");
        }

        [HttpPost] 
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> DeleteComplaint(int id)
        {
            string deleterUser = User.Identity?.Name ?? "System";
            await _complaintService.DeleteExamAsync(id, deleterUser);
            
            return RedirectToAction("StudentComplaints");
        }

        [HttpGet]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> InstructorComplaints(int examId)
        {
            // Fetch complaints for this specific exam
            var complaints = await _complaintService.GetAllByExamIdAsync(examId);
            
            // Store examId to use for the "Back to Analytics" button
            ViewBag.ExamId = examId;
            
            return View(complaints);
        }

        [HttpPost]
        [Authorize(Roles = "Instructor")]
        public async Task<IActionResult> UpdateResponse(int id, int examId, int attemptId, int questionId, string status, string response, int? adjustedScore)
        {
            if (string.IsNullOrWhiteSpace(status))
            {
                return RedirectToAction("InstructorComplaints", new { examId });
            }

            string modifierUser = User.Identity?.Name ?? "Instructor";
            
            // Update the complaint status and add the instructor's response
            await _complaintService.UpdateResponseAsync(id, status, response ?? string.Empty, modifierUser);

            // If approved and points are provided, update the student's grade
            if (status.Equals("Approved", StringComparison.OrdinalIgnoreCase) && adjustedScore.HasValue)
            {
                await _examGradingService.UpdateStudentAnswerGrade(attemptId, questionId, adjustedScore.Value, modifierUser);
            }

            return RedirectToAction("InstructorComplaints", new { examId });
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
}