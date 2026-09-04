using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using QuizEra.BLL.ModelVM.Auth;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;
using System.Security.Claims;

namespace QuizEra.BLL.Services.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Instructor> _instructorRepository;
        private readonly IEmailService _emailService;
        private readonly IConfiguration _configuration;

        public AuthService(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IGenericRepository<Student> studentRepository,
        IGenericRepository<Instructor> instructorRepository,
        IEmailService emailService,
        IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _studentRepository = studentRepository;
            _instructorRepository = instructorRepository;
            _emailService = emailService;
            _configuration = configuration;
        }

        // =========================
        // Register Student
        // =========================

        public async Task<IdentityResult> RegisterStudentAsync(
            RegisterStudentVM model)
        {
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password);

            if (!result.Succeeded)
                return result;

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                "Student");

            if (!roleResult.Succeeded)
                return roleResult;

            var student = new Student(user.Id);

            await _studentRepository.Create(student);
            await _studentRepository.SaveAsync();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var appUrl = _configuration["AppUrl"];

            var confirmationLink =
                $"{appUrl}/Account/ConfirmEmail" +
                $"?userId={Uri.EscapeDataString(user.Id)}" +
                $"&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);

            return result;
        }

        // =========================
        // Register Instructor
        // =========================

        public async Task<bool> RegisterInstructorAsync(
            RegisterInstructorVM model)
        {
            var existingUser =
                await _userManager.FindByEmailAsync(model.Email);

            if (existingUser != null)
                return false;

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password);

            if (!result.Succeeded)
                return false;

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                "Instructor");

            if (!roleResult.Succeeded)
                return false;

            var instructor = new Instructor(user.Id);

            await _instructorRepository.Create(instructor);
            await _instructorRepository.SaveAsync();

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var appUrl = _configuration["AppUrl"];

            var confirmationLink =
                $"{appUrl}/Account/ConfirmEmail" +
                $"?userId={Uri.EscapeDataString(user.Id)}" +
                $"&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailConfirmationAsync(user.Email, confirmationLink);

            return true;
        }

        // =========================
        // Normal Login
        // =========================

        public async Task<LoginResult> LoginAsync(LoginVM model)
        {
            var user =
                await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
            {
                Console.WriteLine("USER NOT FOUND");

                return LoginResult.UserNotFound;
            }

            Console.WriteLine($"USER FOUND: {user.Email}");
            Console.WriteLine($"IS ACTIVE: {user.IsActive}");

            // IMPORTANT
            if (!user.IsActive)
            {
                Console.WriteLine("USER IS DEACTIVATED");

                return LoginResult.Deactivated;
            }

            var result =
                await _signInManager.CheckPasswordSignInAsync(
                    user,
                    model.Password,
                    false);

            Console.WriteLine($"PASSWORD CORRECT: {result.Succeeded}");
            Console.WriteLine($"NOT ALLOWED: {result.IsNotAllowed}");
            Console.WriteLine($"LOCKED OUT: {result.IsLockedOut}");

            if (result.IsLockedOut)
                return LoginResult.LockedOut;

            if (result.IsNotAllowed)
                return LoginResult.NotAllowed;

            if (!result.Succeeded)
                return LoginResult.InvalidPassword;

            await _signInManager.SignInAsync(
                user,
                isPersistent: model.RememberMe);

            return LoginResult.Success;
        }

        // =========================
        // Get User By Email
        // =========================

        public async Task<ApplicationUser?> GetUserByEmailAsync(
            string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        // =========================
        // Get User Role
        // =========================

        public async Task<string?> GetUserRoleAsync(
            string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);

            return roles.FirstOrDefault();
        }

        // =========================
        // External Login
        // Existing User
        // =========================

        public async Task<bool> ExternalLoginAsync(
            ClaimsPrincipal principal,
            string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return false;

            var loginInfo = new UserLoginInfo(
                "Google",
                principal.FindFirstValue(ClaimTypes.NameIdentifier)
                    ?? email,
                "Google");

            var existingLogins =
                await _userManager.GetLoginsAsync(user);

            if (!existingLogins.Any(x =>
                x.LoginProvider == loginInfo.LoginProvider &&
                x.ProviderKey == loginInfo.ProviderKey))
            {
                var result = await _userManager.AddLoginAsync(
                    user,
                    loginInfo);

                if (!result.Succeeded)
                    return false;
            }

            await _signInManager.SignInAsync(
                user,
                isPersistent: false);

            return true;
        }

        // =========================
        // Register External User
        // New User + Selected Role
        // =========================

        public async Task<bool> RegisterExternalUserAsync(
            string email,
            string firstName,
            string lastName,
            string role)
        {
            var existingUser =
                await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
                return false;

            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                FirstName = firstName,
                LastName = lastName,
                EmailConfirmed = true
            };

            // External login doesn't have a password
            var result = await _userManager.CreateAsync(user);

            if (!result.Succeeded)
                return false;

            var roleResult = await _userManager.AddToRoleAsync(
                user,
                role);

            if (!roleResult.Succeeded)
                return false;

            // =========================
            // Student
            // =========================

            if (role == "Student")
            {
                var student = new Student(user.Id);

                await _studentRepository.Create(student);
                await _studentRepository.SaveAsync();
            }

            // =========================
            // Instructor
            // =========================

            else if (role == "Instructor")
            {
                var instructor = new Instructor(user.Id);

                await _instructorRepository.Create(instructor);
                await _instructorRepository.SaveAsync();
            }

            await _signInManager.SignInAsync(
                user,
                isPersistent: false);

            return true;
        }

        public async Task<bool> ConfirmEmailAsync(string userId, string token)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return false;

            var result = await _userManager.ConfirmEmailAsync(user, token);

            return result.Succeeded;
        }

        public async Task<bool> ForgotPasswordAsync(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
                return true;

            var token =
                await _userManager.GeneratePasswordResetTokenAsync(user);

            var resetLink =
                $"https://localhost:7077/Account/ResetPassword" +
                $"?userId={user.Id}" +
                $"&token={Uri.EscapeDataString(token)}";

            await _emailService.SendPasswordResetEmailAsync(user.Email!,resetLink);

            return true;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordVM model)
        {
            var user =
                await _userManager.FindByIdAsync(model.UserId);

            if (user == null)
                return false;

            var result =
                await _userManager.ResetPasswordAsync(user,model.Token, model.Password);

            return result.Succeeded;
        }

        // =========================
        // Logout
        // =========================

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}