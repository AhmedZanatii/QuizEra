using QuizEra.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;
using QuizEra.BLL.ModelVM.Questions;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
   
        /* CRUD
         * Get all questions
           Get question by ID
           Create question
           Update question
           Delete question*/

        public class QuestionService : IQuestionService
        {
            private readonly IGenericRepository<Question> QuestionRepo;
            private readonly IGenericRepository<QuestionOption> OptionRepo;

            public QuestionService(
                IGenericRepository<Question> questionRepo,
                IGenericRepository<QuestionOption> optionRepo)
            {
                QuestionRepo = questionRepo;
                OptionRepo = optionRepo;
            }

            public async Task<IEnumerable<QuestionVM>> GetAllAsync()
            {
                try
                {
                    var questions = await QuestionRepo.Get(
                        includeProperties: new List<System.Linq.Expressions.Expression<Func<Question, object>>>
                        {
                        q => q.Options
                        });

                    return questions.Select(q => new QuestionVM
                    {
                        Id = q.Id,
                        TopicID = q.TopicID,
                        QuestionText = q.QuestionText,
                        QuestionType = q.QuestionType,
                        QuestionFormat = q.QuestionFormat,
                        QuestionAnswer = q.QuestionAnswer,
                        DifficultyLevel = q.DifficultyLevel,
                        Photo = q.Photo,

                        Options = q.Options.Select(o => new QuestionOptionVM
                        {
                            Id = o.Id,
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    });
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving questions: {ex.Message}", ex);
                }
            }

            public async Task<QuestionVM?> GetByIdAsync(int id)
            {
                try
                {
                    var question = (await QuestionRepo.Get(
                        q => q.Id == id,
                        includeProperties: new List<System.Linq.Expressions.Expression<Func<Question, object>>>
                        {
                        q => q.Options
                        }))
                        .FirstOrDefault();

                    if (question == null)
                        return null;

                    return new QuestionVM
                    {
                        Id = question.Id,
                        TopicID = question.TopicID,
                        QuestionText = question.QuestionText,
                        QuestionType = question.QuestionType,
                        QuestionFormat = question.QuestionFormat,
                        QuestionAnswer = question.QuestionAnswer,
                        DifficultyLevel = question.DifficultyLevel,
                        Photo = question.Photo,

                        Options = question.Options.Select(o => new QuestionOptionVM
                        {
                            Id = o.Id,
                            OptionText = o.OptionText,
                            IsCorrect = o.IsCorrect
                        }).ToList()
                    };
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error retrieving question with ID {id}: {ex.Message}", ex);
                }
            }

            public async Task AddAsync(QuestionVM question)
            {
                try
                {
                    ValidateQuestion(question);

                var newQuestion = new Question(
                 question.TopicID,
                 question.QuestionText,
                 question.QuestionType,
                 question.QuestionFormat,
                 question.QuestionAnswer,
                 question.DifficultyLevel,
                 question.Photo
                );

                await QuestionRepo.Create(newQuestion);
                    await QuestionRepo.SaveAsync();

                    if (question.Options != null && question.Options.Any())
                    {
                        foreach (var option in question.Options)
                        {
                            var newOption = new QuestionOption(
                                newQuestion.Id,
                                option.OptionText,
                                option.IsCorrect
                            );

                            await OptionRepo.Create(newOption);
                        }

                        await OptionRepo.SaveAsync();
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception($"Error adding question: {ex.Message}", ex);
                }
            }

            public async Task UpdateAsync(QuestionVM question)
            {
                try
                {
                    ValidateQuestion(question);

                    var existingQuestion = (await QuestionRepo.Get(
                        q => q.Id == question.Id))
                        .FirstOrDefault();

                    if (existingQuestion == null)
                    {
                        throw new Exception(
                            $"Question with ID {question.Id} was not found.");
                    }

                existingQuestion.Update(question.TopicID, question.QuestionText,question.QuestionFormat,
                    question.QuestionType, question.QuestionAnswer, question.DifficultyLevel, question.Photo );

                QuestionRepo.Update(existingQuestion);

                    // Get existing options
                    var existingOptions = (await OptionRepo.Get(
                        o => o.QuestionId == question.Id))
                        .ToList();

                    // Delete old options
                    foreach (var option in existingOptions)
                    {
                        OptionRepo.Delete(option);
                    }

                    // Add new options
                    if (question.Options != null && question.Options.Any())
                    {
                        foreach (var option in question.Options)
                        {
                            var newOption = new QuestionOption(
                                question.Id,
                                option.OptionText,
                                option.IsCorrect
                            );

                            await OptionRepo.Create(newOption);
                        }
                    }

                    await QuestionRepo.SaveAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Error updating question with ID {question.Id}: {ex.Message}", ex);
                }
            }

            public async Task DeleteAsync(int id)
            {
                try
                {
                    var question = (await QuestionRepo.Get(
                        q => q.Id == id))
                        .FirstOrDefault();

                    if (question == null)
                    {
                        throw new Exception(
                            $"Question with ID {id} was not found.");
                    }

                    QuestionRepo.Delete(question);

                    await QuestionRepo.SaveAsync();
                }
                catch (Exception ex)
                {
                    throw new Exception(
                        $"Error deleting question with ID {id}: {ex.Message}", ex);
                }
            }

            private void ValidateQuestion(QuestionVM question)
            {
                if (question == null)
                    throw new ArgumentException("Question cannot be null.");

                if (question.TopicID <= 0)
                    throw new ArgumentException("Topic ID must be greater than zero.");

                if (string.IsNullOrWhiteSpace(question.QuestionText))
                    throw new ArgumentException("Question text is required.");

                // Essay needs a reference answer
                if (question.QuestionFormat ==
                    QuizEra.DAL.Entities.Enums.QuestionFormat.Essay &&
                    string.IsNullOrWhiteSpace(question.QuestionAnswer))
                {
                    throw new ArgumentException(
                        "Question answer is required for essay questions.");
                }

                // MCQ needs options
                if (question.QuestionFormat ==
                    QuizEra.DAL.Entities.Enums.QuestionFormat.MCQ)
                {
                    if (question.Options == null || !question.Options.Any())
                        throw new ArgumentException(
                            "MCQ question must have at least one option.");

                    if (question.Options.Count(o => o.IsCorrect) != 1)
                        throw new ArgumentException(
                            "MCQ question must have exactly one correct option.");
                }

                // True/False needs options
                if (question.QuestionFormat ==
                    QuizEra.DAL.Entities.Enums.QuestionFormat.TrueFalse)
                {
                    if (question.Options == null || question.Options.Count != 2)
                        throw new ArgumentException(
                            "True/False question must have exactly two options.");
                }
            }
        }
  
}
