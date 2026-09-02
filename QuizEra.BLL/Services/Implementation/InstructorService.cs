using Microsoft.AspNetCore.Identity;
using QuizEra.BLL.ModelVM.Administration;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;
using System.Linq.Expressions;

namespace QuizEra.BLL.Services.Implementation
{
    public class InstructorService : IInstructorService
    {
        private readonly IGenericRepository<Instructor> _instructorRepository;
        private readonly IGenericRepository<Course> _courseRepository;
        private readonly IGenericRepository<Topic> _topicRepository;
        private readonly IGenericRepository<StudentCourse> _studentCourseRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public InstructorService(
            IGenericRepository<Instructor> instructorRepository,
            IGenericRepository<Course> courseRepository,
            IGenericRepository<Topic> topicRepository,
            IGenericRepository<StudentCourse> studentCourseRepository,
            UserManager<ApplicationUser> userManager)
        {
            _instructorRepository = instructorRepository;
            _courseRepository = courseRepository;
            _topicRepository = topicRepository;
            _studentCourseRepository = studentCourseRepository;
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

        // =========================
        // Get Instructor Dashboard Stats
        // =========================

        public async Task<InstructorDashboardVM> GetInstructorDashboardStatsAsync(string userId)
        {
            // Get the instructor record by AppUserId
            var instructor = await _instructorRepository.GetBy(
                i => i.AppUserId == userId,
                new List<Expression<Func<Instructor, object>>>
                {
                    i => i.Courses
                });

            var dashboard = new InstructorDashboardVM();

            if (instructor == null)
                return dashboard;

            // Get all courses assigned to this instructor
            var courses = await _courseRepository.Get(
                filter: c => c.InstructorID == instructor.Id && !c.IsDeleted,
                includeProperties: new List<Expression<Func<Course, object>>>
                {
                    c => c.Topics
                }
            );

            dashboard.AssignedCoursesCount = courses.Count();

            if (courses.Any())
            {
                var courseIds = courses.Select(c => c.Id).ToList();

                // Get all topics in the instructor's courses
                var topics = courses.SelectMany(c => c.Topics).ToList();
                var topicIds = topics.Select(t => t.Id).ToList();

                if (topicIds.Any())
                {
                    // Get all exam-topic relationships for these topics
                    var allTopics = await _topicRepository.Get(
                        filter: t => topicIds.Contains(t.Id),
                        includeProperties: new List<Expression<Func<Topic, object>>>
                        {
                            t => t.ExamTopics
                        }
                    );

                    // Count unique exams from all ExamTopic relationships
                    dashboard.TotalExamsCount = allTopics
                        .SelectMany(t => t.ExamTopics)
                        .Select(et => et.ExamId)
                        .Distinct()
                        .Count();
                }
                else
                {
                    dashboard.TotalExamsCount = 0;
                }

                // Get all enrolled students in the instructor's courses
                var enrolledStudents = await _studentCourseRepository.Get(
                    filter: sc => courseIds.Contains(sc.CourseId)
                );

                dashboard.EnrolledStudentsCount = enrolledStudents
                    .Select(sc => sc.StudentId)
                    .Distinct()
                    .Count();
            }

            return dashboard;
        }
    }
}