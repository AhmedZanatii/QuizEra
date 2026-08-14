using Microsoft.AspNetCore.Identity;
using QuizEra.BLL.ModelVM.Auth;
using QuizEra.BLL.Services.Auth.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Auth.Implementation
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        private readonly IGenericRepository<Student> _studentRepository;
        private readonly IGenericRepository<Instructor> _instructorRepository;

        public AuthService(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            IGenericRepository<Student> studentRepository,
            IGenericRepository<Instructor> instructorRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _studentRepository = studentRepository;
            _instructorRepository = instructorRepository;
        }

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

            return result;
        }

        public async Task<bool> LoginAsync(LoginVM model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return false;

            var result = await _signInManager.PasswordSignInAsync(
                user.UserName,
                model.Password,
                false,
                false);

            return result.Succeeded;
        }

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
                EmailConfirmed = true
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

            return true;
        }

        public async Task LogoutAsync()
        {
            await _signInManager.SignOutAsync();
        }
    }
}