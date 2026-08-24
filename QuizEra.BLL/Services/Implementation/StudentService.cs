using Microsoft.AspNetCore.Identity;
using QuizEra.BLL.ModelVM.Administration;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
    public class StudentService : IStudentService
    {
        private readonly IGenericRepository<Student> _studentRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public StudentService(
            IGenericRepository<Student> studentRepository,
            UserManager<ApplicationUser> userManager)
        {
            _studentRepository = studentRepository;
            _userManager = userManager;
        }

        // =========================
        // Get All Students
        // =========================

        public async Task<IEnumerable<StudentModelVM>> GetAllAsync()
        {
            var students = await _studentRepository.Get(
                includeProperties: new List<
                    System.Linq.Expressions.Expression<
                        Func<Student, object>>>
                {
                    s => s.AppUser
                });

            return students.Select(s => new StudentModelVM
            {
                Id = s.Id,
                AppUserId = s.AppUserId,
                FirstName = s.AppUser.FirstName,
                LastName = s.AppUser.LastName,
                Email = s.AppUser.Email!,
                PhoneNumber = s.AppUser.PhoneNumber,
                IsActive = s.AppUser.IsActive
            });
        }

        // =========================
        // Get Student By Id
        // =========================

        public async Task<StudentModelVM?> GetByIdAsync(int id)
        {
            var student = await _studentRepository.GetBy(
                s => s.Id == id,
                new List<
                    System.Linq.Expressions.Expression<
                        Func<Student, object>>>
                {
                    s => s.AppUser
                });

            if (student == null)
                return null;

            return new StudentModelVM
            {
                Id = student.Id,
                AppUserId = student.AppUserId,
                FirstName = student.AppUser.FirstName,
                LastName = student.AppUser.LastName,
                Email = student.AppUser.Email!,
                PhoneNumber = student.AppUser.PhoneNumber,
                IsActive = student.AppUser.IsActive
            };
        }

        // =========================
        // Create Student
        // =========================

        public async Task<bool> CreateAsync(StudentModelVM vm)
        {
            var existingUser =
                await _userManager.FindByEmailAsync(vm.Email);

            if (existingUser != null)
                return false;

            var user = new ApplicationUser
            {
                UserName = vm.Email,
                Email = vm.Email,
                FirstName = vm.FirstName,
                LastName = vm.LastName,

                // New users are active by default
                IsActive = true,

                // Admin creates the account,
                // so we don't need email confirmation here
                EmailConfirmed = true
            };

            var userResult =
                await _userManager.CreateAsync(user, vm.Password);

            if (!userResult.Succeeded)
                return false;

            // Add Student Role
            var roleResult =
                await _userManager.AddToRoleAsync(user, "Student");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return false;
            }

            // Create Student profile
            var student = new Student(user.Id);

            await _studentRepository.Create(student);
            await _studentRepository.SaveAsync();

            return true;
        }

        // =========================
        // Update Student
        // =========================

        public async Task<bool> UpdateAsync(StudentModelVM vm)
        {
            var student = await _studentRepository.GetBy(
                s => s.Id == vm.Id,
                new List<
                    System.Linq.Expressions.Expression<
                        Func<Student, object>>>
                {
                    s => s.AppUser
                });

            if (student == null)
                return false;

            var user = student.AppUser;

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;
            user.PhoneNumber = vm.PhoneNumber;
            // Check if email was changed
            if (user.Email != vm.Email)
            {
                var emailExists =
                    await _userManager.FindByEmailAsync(vm.Email);

                if (emailExists != null &&
                    emailExists.Id != user.Id)
                {
                    return false;
                }

                user.Email = vm.Email;
                user.UserName = vm.Email;
            }

            var result =
                await _userManager.UpdateAsync(user);

            return result.Succeeded;
        }

        // =========================
        // Deactivate Student
        // =========================

        public async Task<bool> DeactivateAsync(int id)
        {
            var student = await _studentRepository.GetBy(
                s => s.Id == id,
                new List<
                    System.Linq.Expressions.Expression<
                        Func<Student, object>>>
                {
                    s => s.AppUser
                });

            if (student == null)
                return false;

            // Soft Delete
            student.AppUser.IsActive = false;

            var result =
                await _userManager.UpdateAsync(student.AppUser);

            return result.Succeeded;
        }

        // =========================
        // Restore Student
        // =========================

        public async Task<bool> RestoreAsync(int id)
        {
            var student = await _studentRepository.GetBy(
                s => s.Id == id,
                new List<
                    System.Linq.Expressions.Expression<
                        Func<Student, object>>>
                {
                    s => s.AppUser
                });

            if (student == null)
                return false;

            // Restore account
            student.AppUser.IsActive = true;

            var result =
                await _userManager.UpdateAsync(student.AppUser);

            return result.Succeeded;
        }
    }
}