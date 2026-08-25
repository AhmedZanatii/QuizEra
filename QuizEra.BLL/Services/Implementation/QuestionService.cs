using QuizEra.BLL.ModelVM.Questions;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;
using System.Linq.Expressions;

namespace QuizEra.BLL.Services.Implementation
{
    public class QuestionService : IQuestionService
    {
        private readonly IGenericRepository<Question> _questionRepo;
        private readonly IGenericRepository<QuestionOption> _optionRepo;

        public QuestionService(
            IGenericRepository<Question> questionRepo,
            IGenericRepository<QuestionOption> optionRepo)
        {
            _questionRepo = questionRepo;
            _optionRepo = optionRepo;
        }


        // =========================================
        // Get All Questions
        // =========================================

        public async Task<IEnumerable<QuestionVM>> GetAllAsync()
        {
            var questions = await _questionRepo.Get(
                q => !q.IsDeleted,
                new List<Expression<Func<Question, object>>>
                {
            q => q.Options,
            q => q.Topic
                });

            return questions.Select(q => new QuestionVM
            {
                Id = q.Id,
                TopicID = q.TopicID,
                TopicName = q.Topic.Name,
                QuestionText = q.QuestionText,
                QuestionFormat = q.QuestionFormat,
                QuestionAnswer = q.QuestionAnswer,
                DifficultyLevel = q.DifficultyLevel,
                Photo = q.Photo,
                IsDeleted = q.IsDeleted,

                Options = q.Options
                    .Select(o => new QuestionOptionVM
                    {
                        Id = o.Id,
                        QuestionId = o.QuestionId,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    })
                    .ToList()
            });
        }

        // =========================================
        // Get Question By ID
        // =========================================

        public async Task<QuestionVM?> GetByIdAsync(int id)
        {
            var questions = await _questionRepo.Get(
                q => q.Id == id && !q.IsDeleted,
                new List<Expression<Func<Question, object>>>
                {
                    q => q.Options,
                    q => q.Topic
                });

            var question = questions.FirstOrDefault();

            if (question == null)
                return null;

            return new QuestionVM
            {
                Id = question.Id,
                TopicID = question.TopicID,
                TopicName = question.Topic.Name,
                QuestionText = question.QuestionText,
                QuestionFormat = question.QuestionFormat,
                QuestionAnswer = question.QuestionAnswer,
                DifficultyLevel = question.DifficultyLevel,
                Photo = question.Photo,

                Options = question.Options
                    .Select(o => new QuestionOptionVM
                    {
                        Id = o.Id,
                        QuestionId = o.QuestionId,
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    })
        .ToList()
            };
        }


        // =========================================
        // Add Question
        // =========================================

        public async Task AddAsync(
            QuestionVM vm,
            string creatorUser)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));

            var question = new Question(
                vm.TopicID,
                vm.QuestionText,
                vm.QuestionFormat,
                vm.QuestionAnswer,
                vm.DifficultyLevel,
                vm.Photo,
                creatorUser);

            await _questionRepo.Create(question);

            await _questionRepo.SaveAsync();


            // =========================================
            // Add Options
            // =========================================

            if (vm.Options != null &&
                vm.Options.Any())
            {
                foreach (var option in vm.Options)
                {
                    var questionOption = new QuestionOption(
                        question.Id,
                        option.OptionText,
                        option.IsCorrect);

                    await _optionRepo.Create(questionOption);
                }

                await _optionRepo.SaveAsync();
            }
        }


        // =========================================
        // Update Question
        // =========================================

        public async Task UpdateAsync(
            QuestionVM vm,
            string modifierUser)
        {
            if (vm == null)
                throw new ArgumentNullException(nameof(vm));

            var questions = await _questionRepo.Get(
                q => q.Id == vm.Id && !q.IsDeleted,
                new List<Expression<Func<Question, object>>>
                {
                    q => q.Options
                });

            var existingQuestion = questions.FirstOrDefault();

            if (existingQuestion == null)
                throw new Exception(
                    $"Question with ID {vm.Id} was not found.");


            // Update Question

            existingQuestion.Update(
                vm.TopicID,
                vm.QuestionText,
                vm.QuestionFormat,
                vm.QuestionAnswer,
                vm.DifficultyLevel,
                vm.Photo,
                modifierUser);

            _questionRepo.Update(existingQuestion);


            // =========================================
            // Update Options
            // =========================================

            if (vm.Options != null)
            {
                foreach (var option in vm.Options)
                {
                    var existingOption =
                        existingQuestion.Options
                            .FirstOrDefault(o => o.Id == option.Id);

                    if (existingOption != null)
                    {
                        existingOption.Update(
                            option.OptionText,
                            option.IsCorrect);

                        _optionRepo.Update(existingOption);
                    }
                    else
                    {
                        var newOption = new QuestionOption(
                            existingQuestion.Id,
                            option.OptionText,
                            option.IsCorrect);

                        await _optionRepo.Create(newOption);
                    }
                }
            }

            await _questionRepo.SaveAsync();
        }


        // =========================================
        // Delete Question
        // =========================================

        public async Task DeleteAsync(
            int id,
            string deleterUser)
        {
            var questions = await _questionRepo.Get(
                q => q.Id == id && !q.IsDeleted);

            var question = questions.FirstOrDefault();

            if (question == null)
                throw new Exception(
                    $"Question with ID {id} was not found.");


            question.Delete(
                deleterUser,
                DateTime.UtcNow);

            _questionRepo.Update(question);

            await _questionRepo.SaveAsync();
        }
        public async Task<IEnumerable<QuestionVM>> GetByIdAsyncIncludingDeleted()
        {
            var questions = await _questionRepo.Get(
                null,
                new List<Expression<Func<Question, object>>>
                {
                    q => q.Options,
                    q => q.Topic
                });

            return questions.Select(question => new QuestionVM
            {
                Id = question.Id,
                TopicID = question.TopicID,
                TopicName = question.Topic?.Name,
                QuestionText = question.QuestionText,
                QuestionFormat = question.QuestionFormat,
                QuestionAnswer = question.QuestionAnswer,
                DifficultyLevel = question.DifficultyLevel,
                Photo = question.Photo,
                IsDeleted = question.IsDeleted,

                Options = question.Options.Select(o => new QuestionOptionVM
                {
                    Id = o.Id,
                    QuestionId = o.QuestionId,
                    OptionText = o.OptionText,
                    IsCorrect = o.IsCorrect
                }).ToList()
            });
        }
        public async Task RestoreAsync(int id)
        {
            var questions = await _questionRepo.Get(
                q => q.Id == id
            );

            var question = questions.FirstOrDefault();

            if (question == null)
                throw new Exception($"Question with ID {id} was not found.");

            question.Restore();

            _questionRepo.Update(question);

            await _questionRepo.SaveAsync();
        }
    }
}