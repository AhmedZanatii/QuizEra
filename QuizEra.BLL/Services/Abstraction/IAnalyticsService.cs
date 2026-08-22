using System.Threading.Tasks;
using QuizEra.BLL.ModelVM.Analytics;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IAnalyticsService
    {
        Task<StudentAnalyticsVM> GetStudentAnalyticsAsync(int studentExamAttemptId);
        Task<ClassAnalyticsVM> GetClassAnalyticsAsync(int examId);

        // Generates the byte array for PDF/Excel exports, implementation will follow
        Task<byte[]> ExportClassReportAsync(int examId, string format);
    }
}