using QuizEra.BLL.ModelVM.Exam;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IExamService
    {
        Task<bool> CreateExamAsync(CreateExamVM model);

        Task<IEnumerable<ExamVM>> GetAllExamsAsync();

        Task<ExamVM?> GetExamByIdAsync(int id);

        Task<bool> UpdateExamAsync(UpdateExamVM model);

        Task<bool> DeleteExamAsync(int id);
    }
}
