using QuizEra.BLL.ModelVM.Questions;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionVM>> GetAllAsync();

        Task<QuestionVM?> GetByIdAsync(int id);

        Task AddAsync(QuestionVM question);

        Task UpdateAsync(QuestionVM question);

        Task DeleteAsync(int id);
    }
}
