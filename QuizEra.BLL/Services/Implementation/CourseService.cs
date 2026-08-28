using QuizEra.BLL.ModelVM.Course;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;
using System.Linq.Expressions;

namespace QuizEra.BLL.Services
{
    public class CourseService : ICourseService
    {
        private readonly IGenericRepository<Course> _courseRepository;
        private readonly IGenericRepository<Instructor> _instructorRepository;
        private readonly IGenericRepository<Student> _studentRepository;

        public CourseService(
            IGenericRepository<Course> courseRepository,
            IGenericRepository<Instructor> instructorRepository,
            IGenericRepository<Student> studentRepository)
        {
            _courseRepository = courseRepository;
            _instructorRepository = instructorRepository;
            _studentRepository = studentRepository;
        }

        public async Task<IEnumerable<CourseVM>> GetAllCoursesAsync()
        {
            var courses = await _courseRepository.Get(
                filter: c => !c.IsDeleted,
                noTrack: true
            );

            return courses.Select(c => new CourseVM
            {
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
                CourseName = course.CourseName,
                CourseLevel = course.CourseLevel,
                CourseCode = course.CourseCode,
                CourseDescription = course.CourseDescription
            };
        }

        public async Task<IEnumerable<CourseVM>> GetCoursesByInstructorAsync(string userId)
        {
            // 1. Resolve Instructor ID using ApplicationUserId string via generic repo
            var instructor = await _instructorRepository.GetBy(
                filter: i => i.AppUserId == userId,
                noTrack: true
            );

            if (instructor == null)
            {
                return Enumerable.Empty<CourseVM>();
            }

            // 2. Fetch courses using the resolved Instructor.Id
            var courses = await _courseRepository.Get(
                filter: c => c.InstructorID == instructor.Id && !c.IsDeleted,
                noTrack: true
            );

            return courses.Select(c => new CourseVM
            {
                Id = c.Id,
                CourseName = c.CourseName,
                CourseLevel = c.CourseLevel,
                CourseCode = c.CourseCode,
                CourseDescription = c.CourseDescription
            });
        }

        public async Task<IEnumerable<CourseVM>> GetCoursesByStudentAsync(string userId)
        {
            // 1. Resolve Student ID using ApplicationUserId string via generic repo
            var student = await _studentRepository.GetBy(
                filter: s => s.AppUserId == userId,
                noTrack: true
            );

            if (student == null)
            {
                return Enumerable.Empty<CourseVM>();
            }

            var includeStudentCourses = new List<Expression<Func<Course, object>>>
            {
                c => c.StudentCourses
            };

            // 2. Fetch courses using the resolved Student.Id
            var courses = await _courseRepository.Get(
                filter: c => !c.IsDeleted && c.StudentCourses.Any(sc => sc.StudentId == student.Id),
                includeProperties: includeStudentCourses,
                noTrack: true
            );

            return courses.Select(c => new CourseVM
            {
                CourseName = c.CourseName,
                CourseLevel = c.CourseLevel,
                CourseCode = c.CourseCode,
                CourseDescription = c.CourseDescription
            });
        }

        public async Task<bool> JoinCourseAsync(Guid courseCode, string studentAppUserId)
        {
            var includeStudentCourses = new List<Expression<Func<Course, object>>>
            {
                c => c.StudentCourses
            };

            var course = await _courseRepository.GetBy(
                filter: c => c.CourseCode == courseCode && !c.IsDeleted,
                includeProperties: includeStudentCourses,
                noTrack: false
            );

            if (course == null)
            {
                return false;
            }

            var student = await _studentRepository.GetBy(
                filter: s => s.AppUserId == studentAppUserId,
                noTrack: true
            );

            if (student == null)
            {
                return false;
            }

            if (course.StudentCourses.Any(sc => sc.StudentId == student.Id))
            {
                return true;
            }

            course.StudentCourses.Add(new StudentCourse(student.Id, course.Id));

            _courseRepository.Update(course);
            await _courseRepository.SaveAsync();

            return true;
        }

        public async Task<bool> CreateCourseAsync(CreateCourseVM createVM)
        {
            var instructor = await _instructorRepository.GetBy(
                filter: i => i.AppUserId == createVM.InstructorId,
                noTrack: true
            );

            if (instructor == null)
            {
                return false;
            }

            var course = new Course(
                instructorID: instructor.Id,
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
                return false;

            var instructor = await _instructorRepository.GetBy(
                filter: i => i.AppUserId == updateVM.InstructorId,
                noTrack: true
            );

            if (instructor == null)
                return false;

            course.ChangeInstructor(instructor.Id);

            course.Update(
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
        public async Task<IEnumerable<CourseVM>> GetAllCoursesIncludingDeletedAsync()
        {
            var courses = await _courseRepository.Get(
                noTrack: true
            );

            return courses.Select(c => new CourseVM
            {
                Id = c.Id,
                CourseName = c.CourseName,
                CourseLevel = c.CourseLevel,
                CourseCode = c.CourseCode,
                CourseDescription = c.CourseDescription,
                IsDeleted = c.IsDeleted
            });
        }

        public async Task<CourseVM?> GetCourseByIdIncludingDeletedAsync(int id)
        {
            var course = await _courseRepository.GetBy(
                filter: c => c.Id == id,
                includeProperties: new List<Expression<Func<Course, object>>>
                {
            c => c.Instructor
                },
                noTrack: true
            );

            if (course == null)
                return null;

            return new CourseVM
            {
                Id = course.Id,
                InstructorId = course.Instructor.AppUserId,
                CourseName = course.CourseName,
                CourseLevel = course.CourseLevel,
                CourseCode = course.CourseCode,
                CourseDescription = course.CourseDescription,
                IsDeleted = course.IsDeleted
            };
        }

        public async Task<bool> RestoreCourseAsync(
            int id,
            string modifierUser)
        {
            var course = await _courseRepository.GetBy(
                filter: c => c.Id == id && c.IsDeleted,
                noTrack: false
            );

            if (course == null)
                return false;

            course.Restore(modifierUser);

            _courseRepository.Update(course);

            await _courseRepository.SaveAsync();

            return true;
        }
    }
}