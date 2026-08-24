using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Administration;
using QuizEra.BLL.Services.Abstraction;

namespace QuizEra.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly IStudentService _studentService;
        private readonly IInstructorService _instructorService;

        public AdminController(
            IStudentService studentService,
            IInstructorService instructorService)
        {
            _studentService = studentService;
            _instructorService = instructorService;
        }

        // =====================================================
        // Admin Dashboard
        // =====================================================

        [HttpGet]
        public IActionResult Index()
        {
            return View();
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
    }
}