using QuizEra.BLL.ModelVM.Course;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories;
using QuizEra.DAL.Repositories.Abstraction;
using System.Linq.Expressions;

namespace QuizEra.BLL.Services
{
    public class CourseService : ICourseService
    {
        private readonly IGenericRepository<Course> _courseRepository;

        public CourseService(IGenericRepository<Course> courseRepository)
        {
            _courseRepository = courseRepository;
        }

        public async Task<IEnumerable<CourseVM>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.Get(
                filter: c => !c.IsDeleted,
                noTrack: true
            );

            return courses.Select(c => new CourseVM
            {
                Id = c.Id,
                InstructorId = c.InstructorID,
                CourseName = c.CourseName,
                CourseLevel = c.CourseLevel,
                CourseCode = c.CourseCode,
                CourseDescription = c.CourseDescription
            });
        }

        public async Task<CourseVM?> GetCourseByIdAsync(int id)
        {
            var course = await _courseRepository.GetBy(
                filter: c => c.Id == id && !c.IsDeleted,
                noTrack: true
            );

            if (course == null) return null;

            return new CourseVM
            {
                Id = course.Id,
                InstructorId = course.InstructorID,
                CourseName = course.CourseName,
                CourseLevel = course.CourseLevel,
                CourseCode = course.CourseCode,
                CourseDescription = course.CourseDescription
               
            };
        }

        public async Task<IEnumerable<CourseVM>> GetCoursesByInstructorAsync(int instructorId)
        {
            var courses = await _courseRepository.Get(
                filter: c => c.InstructorID == instructorId && !c.IsDeleted,
                noTrack: true
            );

            return courses.Select(c => new CourseVM
            {
                Id = c.Id,
                InstructorId = c.InstructorID,
                CourseName = c.CourseName,
                CourseLevel = c.CourseLevel,
                CourseCode = c.CourseCode,
                CourseDescription = c.CourseDescription
                
            });
        }

        public async Task<IEnumerable<CourseVM>> GetCoursesByStudentAsync(int studentId)
        {
            var includeStudentCourses = new List<Expression<Func<Course, object>>>
            {
                c => c.StudentCourses
            };

            var courses = await _courseRepository.Get(
                filter: c => !c.IsDeleted && c.StudentCourses.Any(sc => sc.StudentId == studentId),
                includeProperties: includeStudentCourses,
                noTrack: true
            );

            return courses.Select(c => new CourseVM
            {
                Id = c.Id,
                InstructorId = c.InstructorID,
                CourseName = c.CourseName,
                CourseLevel = c.CourseLevel,
                CourseCode = c.CourseCode,
                CourseDescription = c.CourseDescription
               
            });
        }

        public async Task<bool> CreateCourseAsync(CreateCourseVM createVM)
        {
            var course = new Course(
                instructorID: createVM.InstructorId,
                courseName: createVM.CourseName,
                courseLevel: createVM.CourseLevel,
                courseDescription: createVM.CourseDescription,
                creatorUser: createVM.CreatorUser
            );

            await _courseRepository.Create(course);
            await _courseRepository.SaveAsync();
            return true;

        }

        public async Task<bool> UpdateCourseAsync(UpdateCourseVM updateVM)
        {
            var course = await _courseRepository.GetBy(
                filter: c => c.Id == updateVM.Id && !c.IsDeleted,
                noTrack: false
            );

            if (course == null)
            {
                return false;
            }

            course.Update(
                instructorID: updateVM.InstructorId,
                courseName: updateVM.CourseName,
                courseLevel: updateVM.CourseLevel,
                courseDescription: updateVM.CourseDescription,
                modifierUser: updateVM.ModifierUser
            );

            _courseRepository.Update(course);
            await _courseRepository.SaveAsync();

            return true;
        }

        public async Task<bool> DeleteCourseAsync(int id, string deleterUser)
        {
            var course = await _courseRepository.GetBy(
                filter: c => c.Id == id && !c.IsDeleted,
                noTrack: false
            );

            if (course == null)
            {
                return false;
            }

            bool isDeleted = course.Delete(deleterUser, DateTime.UtcNow);

            if (isDeleted)
            {
                _courseRepository.Update(course);
                await _courseRepository.SaveAsync();
            }

            return isDeleted;
        }
    }
}