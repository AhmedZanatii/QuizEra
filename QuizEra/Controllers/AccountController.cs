using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.DTOs.Auth;
using QuizEra.BLL.Services.Auth;
using QuizEra.DAL.Entities;

namespace QuizEra.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthService _authService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        public AccountController(
      AuthService authService,
      SignInManager<ApplicationUser> signInManager)
        {
            _authService = authService;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterStudentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return View(dto);
            }

            var result = await _authService.RegisterStudentAsync(dto);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                return View(dto);
            }

            return RedirectToAction("Login");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.LoginAsync(dto);

            if (!result)
            {
                ModelState.AddModelError("", "Invalid email or password.");
                return View(dto);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }


        [HttpGet]
        public IActionResult RegisterInstructor()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterInstructor(
            RegisterInstructorDto dto)
        {
            if (!ModelState.IsValid)
                return View(dto);

            var result = await _authService.RegisterInstructorAsync(dto);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Registration failed. Email may already exist.");

                return View(dto);
            }

            return RedirectToAction("Login");
        }
    
    [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login");
        }
    }
}