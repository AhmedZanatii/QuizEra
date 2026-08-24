using Microsoft.AspNetCore.Identity;
using QuizEra.BLL.ModelVM.Administration;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
    public class InstructorService : IInstructorService
    {
        private readonly IGenericRepository<Instructor> _instructorRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorService(
            IGenericRepository<Instructor> instructorRepository,
            UserManager<ApplicationUser> userManager)
        {
            _instructorRepository = instructorRepository;
            _userManager = userManager;
        }

        // =========================
        // Get All Instructors
        // =========================

        public async Task<IEnumerable<InstructorModelVM>> GetAllAsync()
        {
            var instructors = await _instructorRepository.Get(
                includeProperties: new List<
                    System.Linq.Expressions.Expression<
                        Func<Instructor, object>>>
                {
                    i => i.AppUser
                });

            return instructors.Select(i => new InstructorModelVM
            {
                Id = i.Id,
                AppUserId = i.AppUserId,
                FirstName = i.AppUser.FirstName,
                LastName = i.AppUser.LastName,
                Email = i.AppUser.Email!,
                IsActive = i.AppUser.IsActive
            });
        }

        // =========================
        // Get Instructor By Id
        // =========================

        public async Task<InstructorModelVM?> GetByIdAsync(int id)
        {
            var instructor = await _instructorRepository.GetBy(
                i => i.Id == id,
                new List<
                    System.Linq.Expressions.Expression<
                        Func<Instructor, object>>>
                {
                    i => i.AppUser
                });

            if (instructor == null)
                return null;

            return new InstructorModelVM
            {
                Id = instructor.Id,
                AppUserId = instructor.AppUserId,
                FirstName = instructor.AppUser.FirstName,
                LastName = instructor.AppUser.LastName,
                Email = instructor.AppUser.Email!,
                IsActive = instructor.AppUser.IsActive
            };
        }

        // =========================
        // Create Instructor
        // =========================

        public async Task<bool> CreateAsync(InstructorModelVM vm)
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
                IsActive = true,
                EmailConfirmed = true
            };

            var userResult =
                await _userManager.CreateAsync(user, vm.Password);

            if (!userResult.Succeeded)
                return false;

            var roleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    "Instructor");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);
                return false;
            }

            var instructor = new Instructor(user.Id);

            await _instructorRepository.Create(instructor);
            await _instructorRepository.SaveAsync();

            return true;
        }

        // =========================
        // Update Instructor
        // =========================

        public async Task<bool> UpdateAsync(InstructorModelVM vm)
        {
            var instructor = await _instructorRepository.GetBy(
                i => i.Id == vm.Id,
                new List<
                    System.Linq.Expressions.Expression<
                        Func<Instructor, object>>>
                {
                    i => i.AppUser
                });

            if (instructor == null)
                return false;

            var user = instructor.AppUser;

            user.FirstName = vm.FirstName;
            user.LastName = vm.LastName;

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
        // Deactivate Instructor
        // =========================

        public async Task<bool> DeactivateAsync(int id)
        {
            var instructor = await _instructorRepository.GetBy(
                i => i.Id == id,
                new List<
                    System.Linq.Expressions.Expression<
                        Func<Instructor, object>>>
                {
                    i => i.AppUser
                });

            if (instructor == null)
                return false;

            instructor.AppUser.IsActive = false;

            var result =
                await _userManager.UpdateAsync(
                    instructor.AppUser);

            return result.Succeeded;
        }

        // =========================
        // Restore Instructor
        // =========================

        public async Task<bool> RestoreAsync(int id)
        {
            var instructor = await _instructorRepository.GetBy(
                i => i.Id == id,
                new List<
                    System.Linq.Expressions.Expression<
                        Func<Instructor, object>>>
                {
                    i => i.AppUser
                });

            if (instructor == null)
                return false;

            instructor.AppUser.IsActive = true;

            var result =
                await _userManager.UpdateAsync(
                    instructor.AppUser);

            return result.Succeeded;
        }
    }
}