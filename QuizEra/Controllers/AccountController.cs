using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Auth;
using QuizEra.BLL.ModelVMs.Auth;
using QuizEra.BLL.Services.Abstraction;

namespace QuizEra.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAuthService _authService;

        public AccountController(IAuthService authService)
        {
            _authService = authService;
        }

        // =========================
        // Register Student
        // =========================

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterStudentVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _authService.RegisterStudentAsync(vm);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(vm);
            }

            return RedirectToAction(
                nameof(ConfirmYourEmail),
                new { email = vm.Email });
        }


        // =========================
        // Confirm Your Email Page
        // =========================

        [HttpGet]
        public IActionResult ConfirmYourEmail(string email)
        {
            ViewBag.Email = email;

            return View();
        }


        // =========================
        // Login
        // =========================

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result = await _authService.LoginAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(vm);
            }

            return RedirectToAction("Index", "Home");
        }


        // =========================
        // Access Denied
        // =========================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        // =========================
        // Register Instructor
        // =========================

        [HttpGet]
        public IActionResult RegisterInstructor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterInstructor(
            RegisterInstructorVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result =
                await _authService.RegisterInstructorAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Registration failed. Email may already exist.");

                return View(vm);
            }

            return RedirectToAction(
                nameof(ConfirmYourEmail),
                new { email = vm.Email });
        }


        // =========================
        // Confirm Email
        // =========================

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(
            ConfirmEmailVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result =
                await _authService.ConfirmEmailAsync(
                    vm.UserId!,
                    vm.Token!);

            if (!result)
            {
                return View("EmailConfirmationFailed");
            }

            return View("EmailConfirmed");
        }


        // =========================
        // Logout
        // =========================

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            return RedirectToAction("Login");
        }
    }
}