using System;
using System.Collections.Generic;
using System.Text;

using QuizEra.BLL.ModelVM.Exam;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;
using System.Linq.Expressions;

namespace QuizEra.BLL.Services.Implementation
{
    public class ExamService : IExamService
    {
        private readonly IGenericRepository<Exam> _examRepository;
        private readonly IGenericRepository<ExamQuestions> _examQuestionsRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Topic> _topicRepository;

        public ExamService(
            IGenericRepository<Exam> examRepository,
            IGenericRepository<ExamQuestions> examQuestionsRepository,
            IGenericRepository<Question> questionRepository,
            IGenericRepository<Topic> topicRepository)
        {
            _examRepository = examRepository;
            _examQuestionsRepository = examQuestionsRepository;
            _questionRepository = questionRepository;
            _topicRepository = topicRepository;
        }

        // =========================
        // Create Exam
        // =========================


        public async Task<bool> CreateExamAsync(CreateExamVM model)
        {
            // =========================
            // Validate Dates
            // =========================

            if (model.StartDate >= model.EndDate)
                return false;


            // =========================
            // Validate Topic
            // =========================

            var topic = await _topicRepository.GetBy(
                t => t.Id == model.TopicId);

            if (topic == null)
                return false;


            // =========================
            // Validate Topic belongs to Course
            // =========================

            if (topic.CourseID != model.CourseId)
                return false;


            // =========================
            // Get Selected Questions
            // =========================

            var selectedQuestions = model.Questions
                .Where(q => q.IsSelected)
                .ToList();


            // Exam must contain at least one question

            if (!selectedQuestions.Any())
                return false;


            // =========================
            // Validate Questions
            // =========================

            foreach (var questionVM in selectedQuestions)
            {
                var question = await _questionRepository.GetBy(
                    q => q.Id == questionVM.QuestionId);

                if (question == null)
                    return false;


                // Question must belong to selected topic

                if (question.TopicID != model.TopicId)
                    return false;


                // Actual mark must be greater than zero

                if (questionVM.ActualMark <= 0)
                    return false;


                // Bonus mark cannot be negative

                if (questionVM.BonusMark < 0)
                    return false;
            }


            // =========================
            // Calculate Total Marks
            // =========================

            double totalMarks = selectedQuestions
                .Sum(q => q.ActualMark);


            // =========================
            // Create Exam
            // =========================

            var exam = new Exam(
                model.TopicId,
                model.Title,
                model.Duration,
                totalMarks,
                model.StartDate,
                model.EndDate
            );


            await _examRepository.Create(exam);

            await _examRepository.SaveAsync();


            // =========================
            // Create Exam Questions
            // =========================

            foreach (var questionVM in selectedQuestions)
            {
                var examQuestion = new ExamQuestions(
                    questionVM.QuestionId,
                    exam.Id,
                    questionVM.ActualMark,
                    questionVM.BonusMark,
                    0
                );

                await _examQuestionsRepository.Create(examQuestion);
            }


            await _examQuestionsRepository.SaveAsync();


            return true;
        }

        // =========================
        // Get All Exams
        // =========================

        public async Task<IEnumerable<ExamVM>> GetAllExamsAsync()
        {
            var includes = new List<Expression<Func<Exam, object>>>
            {
                e => e.Topic,
                e => e.ExamQuestions
            };

            var exams = await _examRepository.Get(
                includeProperties: includes);

            return exams.Select(e => new ExamVM
            {
                Id = e.Id,
                TopicId = e.TopicID,
                TopicName = e.Topic?.Name ?? string.Empty,
                Title = e.Title,
                Duration = e.Duration,
                TotalMarks = e.TotalMarks,
                StartDate = e.StartDate,
                EndDate = e.EndDate
            });
        }

        // =========================
        // Get Exam By Id
        // =========================

        public async Task<ExamVM?> GetExamByIdAsync(int id)
        {
            var includes = new List<Expression<Func<Exam, object>>>
    {
        e => e.Topic,
        e => e.ExamQuestions
    };

            var exam = await _examRepository.GetBy(
                e => e.Id == id,
                includes);

            if (exam == null)
                return null;

            var model = new ExamVM
            {
                Id = exam.Id,
                CourseId = exam.Topic?.CourseID ?? 0,
                CourseName = exam.Topic?.Course?.CourseName ?? string.Empty,
                TopicId = exam.TopicID,
                TopicName = exam.Topic?.Name ?? string.Empty,
                Title = exam.Title,
                Duration = exam.Duration,
                TotalMarks = exam.TotalMarks,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate
            };

            foreach (var examQuestion in exam.ExamQuestions)
            {
                var question = await _questionRepository.GetBy(
                    q => q.Id == examQuestion.QuestionId);

                if (question == null)
                    continue;

                model.Questions.Add(new CreateExamQuestionVM
                {
                    ExamQuestionId = examQuestion.Id,
                    QuestionId = question.Id,
                    QuestionText = question.QuestionText,
                    IsSelected = true,
                    ActualMark = examQuestion.ActualMark,
                    BonusMark = examQuestion.BonusMark
                });
            }

            return model;
        }
        // =========================
        // Update Exam
        // =========================

        public async Task<bool> UpdateExamAsync(UpdateExamVM model)
        {
            // Validate dates
            if (model.StartDate >= model.EndDate)
                return false;

            // Get the existing exam with its questions
            var includes = new List<Expression<Func<Exam, object>>>
    {
        e => e.ExamQuestions
    };

            var exam = await _examRepository.GetBy(
                e => e.Id == model.Id,
                includes);

            if (exam == null)
                return false;

            // Validate topic
            var topic = await _topicRepository.GetBy(
                t => t.Id == model.TopicId);

            if (topic == null)
                return false;

            // Get selected questions from the submitted form
            var selectedQuestions = model.Questions
                .Where(q => q.IsSelected)
                .ToList();

            // Exam must contain at least one question
            if (!selectedQuestions.Any())
                return false;

            // Validate selected questions
            foreach (var questionVM in selectedQuestions)
            {
                var question = await _questionRepository.GetBy(
                    q => q.Id == questionVM.QuestionId);

                if (question == null)
                    return false;

                if (questionVM.ActualMark <= 0)
                    return false;

                if (questionVM.BonusMark < 0)
                    return false;
            }

            // =========================================
            // Synchronize ExamQuestions
            // =========================================

            var existingQuestions = exam.ExamQuestions.ToList();

            // 1. Remove questions that are no longer selected
            foreach (var existingQuestion in existingQuestions)
            {
                bool stillSelected = selectedQuestions.Any(
                    q => q.QuestionId == existingQuestion.QuestionId);

                if (!stillSelected)
                {
                    _examQuestionsRepository.Delete(existingQuestion);
                }
            }

            // 2. Add new questions / update existing questions
            foreach (var questionVM in selectedQuestions)
            {
                var existingQuestion = existingQuestions.FirstOrDefault(
                    q => q.QuestionId == questionVM.QuestionId);

                if (existingQuestion == null)
                {
                    // New question added to the exam
                    var newExamQuestion = new ExamQuestions(
                        questionVM.QuestionId,
                        exam.Id,
                        questionVM.ActualMark,
                        questionVM.BonusMark,
                        0 // NegativeMark for now
                    );

                    await _examQuestionsRepository.Create(newExamQuestion);
                }
                else
                {
                    // Existing question: update its marks
                    existingQuestion.Update(
                        questionVM.ActualMark,
                        questionVM.BonusMark,
                        existingQuestion.NegativeMark
                    );

                    _examQuestionsRepository.Update(existingQuestion);
                }
            }

            // =========================================
            // Recalculate Total Marks
            // =========================================

            double totalMarks = selectedQuestions
                .Sum(q => q.ActualMark);

            // Update the Exam itself
            exam.Update(
                model.Title,
                model.Duration,
                totalMarks,
                model.TopicId,
                model.StartDate,
                model.EndDate
            );

            _examRepository.Update(exam);

            // Save changes
            await _examQuestionsRepository.SaveAsync();
            await _examRepository.SaveAsync();

            return true;
        }

        // =========================
        // Delete Exam
        // =========================

        public async Task<bool> DeleteExamAsync(int id)
        {
            var exam = await _examRepository.GetBy(
                e => e.Id == id);

            if (exam == null)
                return false;

            _examRepository.Delete(exam);

            await _examRepository.SaveAsync();

            return true;
        }
    }
}