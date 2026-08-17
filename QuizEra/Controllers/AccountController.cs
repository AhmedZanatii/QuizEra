using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using QuizEra.BLL.ModelVM.Auth;
using QuizEra.BLL.Services.Abstraction;
using System.Security.Claims;

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
        // Register
        // =========================

        [HttpGet]
        public IActionResult Register()
        {
            TempData.Remove("IsExternalRegister");
            TempData.Remove("ExternalEmail");
            TempData.Remove("ExternalFirstName");
            TempData.Remove("ExternalLastName");
            return RedirectToAction(nameof(ChooseRole));
        }

        // =========================
        // Choose Role
        // =========================

        [HttpGet]
        public IActionResult ChooseRole()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> ChooseRole(string role)
        {
            if (role != "Student" && role != "Instructor")
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Please choose a valid role.");

                return View();
            }

            // ==========================================
            // Check if this is a Google External Register
            // ==========================================

            var isExternalRegister =
                TempData["IsExternalRegister"] as bool? ?? false;

            if (isExternalRegister)
            {
                var externalEmail =
                    TempData["ExternalEmail"]?.ToString();

                var firstName =
                    TempData["ExternalFirstName"]?.ToString() ?? "";

                var lastName =
                    TempData["ExternalLastName"]?.ToString() ?? "";

                if (string.IsNullOrEmpty(externalEmail))
                {
                    return RedirectToAction(nameof(Register));
                }

                var result =
                    await _authService.RegisterExternalUserAsync(
                        externalEmail,
                        firstName,
                        lastName,
                        role);

                if (!result)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        "Unable to create your account.");

                    TempData.Keep("IsExternalRegister");
                    TempData.Keep("ExternalEmail");
                    TempData.Keep("ExternalFirstName");
                    TempData.Keep("ExternalLastName");

                    return View();
                }

                if (role == "Student")
                {
                    return RedirectToAction("Index", "Student");
                }

                return RedirectToAction("Index", "Instructor");
            }

            // ==========================================
            // Normal Registration
            // ==========================================

            if (role == "Student")
            {
                return RedirectToAction(nameof(RegisterStudent));
            }

            return RedirectToAction(nameof(RegisterInstructor));
        }

        // =========================
        // Register Student
        // =========================

        [HttpGet]
        public IActionResult RegisterStudent()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> RegisterStudent(
            RegisterStudentVM vm)
        {
            if (!ModelState.IsValid)
                return View(vm);

            var result =
                await _authService.RegisterStudentAsync(vm);

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
                return View(vm);

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
        // Confirm Your Email Page
        // =========================

        [HttpGet]
        public IActionResult ConfirmYourEmail(string email)
        {
            ViewBag.Email = email;

            return View();
        }

        // =========================
        // Confirm Email
        // =========================

        [HttpGet]
        public async Task<IActionResult> ConfirmEmail(ConfirmEmailVM vm)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var result =
                await _authService.ConfirmEmailAsync(vm.UserId!,vm.Token!);

            if (!result)
            {
                return View("EmailConfirmationFailed");
            }

            return View("EmailConfirmed");
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
                return View(vm);

            var result =
                await _authService.LoginAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");

                return View(vm);
            }

            var user =
                await _authService.GetUserByEmailAsync(vm.Email);

            if (user == null)
                return RedirectToAction(nameof(Login));

            var role =
                await _authService.GetUserRoleAsync(user.Id);

            if (role == "Student")
            {
                return RedirectToAction("Index", "Student");
            }

            if (role == "Instructor")
            {
                return RedirectToAction("Index", "Instructor");
            }

            return RedirectToAction(nameof(Login));
        }

        // =========================
        // External Login
        // =========================

        [HttpGet]
        public IActionResult ExternalLogin(string provider)
        {
            var redirectUrl = Url.Action(
                nameof(ExternalLoginCallback),
                "Account");

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, provider);
        }

        // =========================
        // External Login Callback
        // =========================

        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback()
        {
            var result =
                await HttpContext.AuthenticateAsync(
                    IdentityConstants.ExternalScheme);

            if (!result.Succeeded ||
                result.Principal == null)
            {
                return RedirectToAction(nameof(Login));
            }

            var principal = result.Principal;

            var email =
                principal.FindFirst(ClaimTypes.Email)?.Value;

            if (string.IsNullOrEmpty(email))
            {
                await HttpContext.SignOutAsync(
                    IdentityConstants.ExternalScheme);

                return RedirectToAction(nameof(Login));
            }

            // ==========================================
            // Check Existing User
            // ==========================================

            var existingUser =
                await _authService.GetUserByEmailAsync(email);

            // ==========================================
            // Existing User
            // ==========================================

            if (existingUser != null)
            {
                var role =
                    await _authService.GetUserRoleAsync(
                        existingUser.Id);

                var loginResult =
                    await _authService.ExternalLoginAsync(
                        principal,
                        email);

                await HttpContext.SignOutAsync(
                    IdentityConstants.ExternalScheme);

                if (!loginResult)
                {
                    return RedirectToAction(nameof(Login));
                }

                if (role == "Student")
                {
                    return RedirectToAction(
                        "Index",
                        "Student");
                }

                if (role == "Instructor")
                {
                    return RedirectToAction(
                        "Index",
                        "Instructor");
                }

                return RedirectToAction(nameof(Login));
            }

            // ==========================================
            // New Google User
            // ==========================================

            var firstName =
                principal.FindFirst(
                    ClaimTypes.GivenName)?.Value ?? "";

            var lastName =
                principal.FindFirst(
                    ClaimTypes.Surname)?.Value ?? "";

            TempData["IsExternalRegister"] = true;
            TempData["ExternalEmail"] = email;
            TempData["ExternalFirstName"] = firstName;
            TempData["ExternalLastName"] = lastName;
            await HttpContext.SignOutAsync(
                IdentityConstants.ExternalScheme);

            return RedirectToAction(nameof(ChooseRole));
        }

        // =========================
        // Logout
        // =========================

        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _authService.LogoutAsync();

            TempData.Remove("IsExternalRegister");
            TempData.Remove("ExternalEmail");
            TempData.Remove("ExternalFirstName");
            TempData.Remove("ExternalLastName");

            return RedirectToAction(nameof(Login));
        }

        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ForgotPasswordConfirmation()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            await _authService.ForgotPasswordAsync(vm.Email);

            return RedirectToAction(
                nameof(ForgotPasswordConfirmation));
        }

        [HttpGet]
        public IActionResult ResetPasswordConfirmation()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ResetPassword(string userId, string token)
        {
            if (string.IsNullOrEmpty(userId) ||
                string.IsNullOrEmpty(token))
            {
                return BadRequest();
            }

            var vm = new ResetPasswordVM
            {
                UserId = userId,
                Token = token
            };

            return View(vm);
        }
        [HttpPost]
        public async Task<IActionResult> ResetPassword(ResetPasswordVM vm)
        {
            if (!ModelState.IsValid)
            {
                return View(vm);
            }

            var result =
                await _authService.ResetPasswordAsync(vm);

            if (!result)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "The password reset link is invalid or expired.");

                return View(vm);
            }

            return RedirectToAction(
                nameof(ResetPasswordConfirmation));
        }


        // =========================
        // Access Denied
        // =========================

        [HttpGet]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
