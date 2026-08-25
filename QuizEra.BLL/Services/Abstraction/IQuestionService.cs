using QuizEra.BLL.ModelVM.Questions;
using System;
using System.Collections.Generic;
using System.Text;

using QuizEra.BLL.ModelVM.Questions;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface IQuestionService
    {
        Task<IEnumerable<QuestionVM>> GetAllAsync();

        Task<QuestionVM?> GetByIdAsync(int id);

        Task AddAsync(QuestionVM question, string creatorUser);

        Task UpdateAsync(QuestionVM question, string modifierUser);

        Task DeleteAsync(int id, string deleterUser);
        Task<IEnumerable<QuestionVM>> GetByIdAsyncIncludingDeleted();

        Task RestoreAsync(int id);

    }
}
