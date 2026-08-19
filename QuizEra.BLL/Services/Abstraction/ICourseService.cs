using QuizEra.BLL.ModelVM.Course;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface ICourseService
    {
        Task<IEnumerable<CourseVM>> GetAllCoursesAsync();
        Task<CourseVM?> GetCourseByIdAsync(int id);
        Task<IEnumerable<CourseVM>> GetCoursesByInstructorAsync(int instructorId);
        Task<IEnumerable<CourseVM>> GetCoursesByStudentAsync(int studentId);
        Task<bool> CreateCourseAsync(CreateCourseVM createVM);
        Task<bool> UpdateCourseAsync(UpdateCourseVM updateVM);
        Task<bool> DeleteCourseAsync(int id, string deleterUser);
    }
}
