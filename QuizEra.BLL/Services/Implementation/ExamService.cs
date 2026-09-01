using System;
using System.Collections.Generic;
using System.Text;

//using QuizEra.BLL.ModelVM.Exam;
//using QuizEra.BLL.Services.Abstraction;
//using QuizEra.DAL.Entities;
//using QuizEra.DAL.Repositories.Abstraction;
//using System.Linq.Expressions;

//namespace QuizEra.BLL.Services.Implementation
//{
//    public class ExamService : IExamService
//    {
//        private readonly IGenericRepository<Exam> _examRepository;
//        private readonly IGenericRepository<ExamQuestions> _examQuestionsRepository;
//        private readonly IGenericRepository<Question> _questionRepository;
//        private readonly IGenericRepository<Topic> _topicRepository;
//        private readonly IGenericRepository<ExamTopic> _examTopicRepository;

//        public ExamService(
//     IGenericRepository<Exam> examRepository,
//     IGenericRepository<ExamQuestions> examQuestionsRepository,
//     IGenericRepository<Question> questionRepository,
//     IGenericRepository<Topic> topicRepository,
//     IGenericRepository<ExamTopic> examTopicRepository)
//        {
//            _examRepository = examRepository;
//            _examQuestionsRepository = examQuestionsRepository;
//            _questionRepository = questionRepository;
//            _topicRepository = topicRepository;
//            _examTopicRepository = examTopicRepository;
//        }

//        // =========================
//        // Create Exam
//        // =========================

//        public async Task<bool> CreateExamAsync(CreateExamVM model)
//        {
//            // Validate schedule
//            if (model.StartDate >= model.EndDate)
//                return false;

//            // Validate topic
//            var topic = await _topicRepository.GetBy(
//                t => t.Id == model.TopicId);

//            if (topic == null)
//                return false;

//            // Get selected questions
//            var selectedQuestions = model.Questions
//                .Where(q => q.IsSelected)
//                .ToList();

//            // An exam should contain at least one question
//            if (!selectedQuestions.Any())
//                return false;

//            // Validate that all selected questions exist
//            foreach (var questionVM in selectedQuestions)
//            {
//                var question = await _questionRepository.GetBy(
//                    q => q.Id == questionVM.QuestionId);

//                if (question == null)
//                    return false;

//                if (questionVM.ActualMark <= 0)
//                    return false;

//                if (questionVM.BonusMark < 0)
//                    return false;
//            }

//            // Calculate total marks
//            double totalMarks = selectedQuestions
//                .Sum(q => q.ActualMark);

//            // Create Exam
//            var exam = new Exam(

//                model.Title,
//                model.Duration,
//                totalMarks,
//                model.StartDate,
//                model.EndDate
//            );

//            await _examRepository.Create(exam);
//            await _examRepository.SaveAsync();

//            // Create ExamQuestions
//            foreach (var questionVM in selectedQuestions)
//            {
//                var examQuestion = new ExamQuestions(
//                    questionVM.QuestionId,
//                    exam.Id,
//                    questionVM.ActualMark,
//                    questionVM.BonusMark,
//                    0 // NegativeMark for now
//                );

//                await _examQuestionsRepository.Create(examQuestion);
//            }

//            await _examQuestionsRepository.SaveAsync();

//            return true;
//        }

//        // =========================
//        // Get All Exams
//        // =========================

//        public async Task<IEnumerable<ExamVM>> GetAllExamsAsync()
//        {
//            var includes = new List<Expression<Func<Exam, object>>>
//            {

//                e => e.ExamQuestions
//            };

//            var exams = await _examRepository.Get(
//                includeProperties: includes);

//            return exams.Select(e => new ExamVM
//            {
//                Id = e.Id,
//                TopicId = e.TopicID,
//                TopicName = e.Topic?.Name ?? string.Empty,
//                Title = e.Title,
//                Duration = e.Duration,
//                TotalMarks = e.TotalMarks,
//                StartDate = e.StartDate,
//                EndDate = e.EndDate
//            });
//        }

//        // =========================
//        // Get Exam By Id
//        // =========================

//        public async Task<ExamVM?> GetExamByIdAsync(int id)
//        {
//            var includes = new List<Expression<Func<Exam, object>>>
//    {
//        e => e.Topic,
//        e => e.ExamQuestions
//    };

//            var exam = await _examRepository.GetBy(
//                e => e.Id == id,
//                includes);

//            if (exam == null)
//                return null;

//            var model = new ExamVM
//            {
//                Id = exam.Id,
//                CourseId = exam.Topic?.CourseID ?? 0,
//                CourseName = exam.Topic?.Course?.CourseName ?? string.Empty,
//                TopicId = exam.TopicID,
//                TopicName = exam.Topic?.Name ?? string.Empty,
//                Title = exam.Title,
//                Duration = exam.Duration,
//                TotalMarks = exam.TotalMarks,
//                StartDate = exam.StartDate,
//                EndDate = exam.EndDate
//            };

//            foreach (var examQuestion in exam.ExamQuestions)
//            {
//                var question = await _questionRepository.GetBy(
//                    q => q.Id == examQuestion.QuestionId);

//                if (question == null)
//                    continue;

//                model.Questions.Add(new CreateExamQuestionVM
//                {
//                    QuestionId = question.Id,
//                    QuestionText = question.QuestionText,
//                    IsSelected = true,
//                    ActualMark = examQuestion.ActualMark,
//                    BonusMark = examQuestion.BonusMark
//                });
//            }

//            return model;
//        }
//        // =========================
//        // Update Exam
//        // =========================

//        public async Task<bool> UpdateExamAsync(UpdateExamVM model)
//        {
//            // Validate dates
//            if (model.StartDate >= model.EndDate)
//                return false;

//            // Get the existing exam with its questions
//            var includes = new List<Expression<Func<Exam, object>>>
//    {
//        e => e.ExamQuestions
//    };

//            var exam = await _examRepository.GetBy(
//                e => e.Id == model.Id,
//                includes);

//            if (exam == null)
//                return false;

//            // Validate topic
//            var topic = await _topicRepository.GetBy(
//                t => t.Id == model.TopicId);

//            if (topic == null)
//                return false;

//            // Get selected questions from the submitted form
//            var selectedQuestions = model.Questions
//                .Where(q => q.IsSelected)
//                .ToList();

//            // Exam must contain at least one question
//            if (!selectedQuestions.Any())
//                return false;

//            // Validate selected questions
//            foreach (var questionVM in selectedQuestions)
//            {
//                var question = await _questionRepository.GetBy(
//                    q => q.Id == questionVM.QuestionId);

//                if (question == null)
//                    return false;

//                if (questionVM.ActualMark <= 0)
//                    return false;

//                if (questionVM.BonusMark < 0)
//                    return false;
//            }

//            // =========================================
//            // Synchronize ExamQuestions
//            // =========================================

//            var existingQuestions = exam.ExamQuestions.ToList();

//            // 1. Remove questions that are no longer selected
//            foreach (var existingQuestion in existingQuestions)
//            {
//                bool stillSelected = selectedQuestions.Any(
//                    q => q.QuestionId == existingQuestion.QuestionId);

//                if (!stillSelected)
//                {
//                    _examQuestionsRepository.Delete(existingQuestion);
//                }
//            }

//            // 2. Add new questions / update existing questions
//            foreach (var questionVM in selectedQuestions)
//            {
//                var existingQuestion = existingQuestions.FirstOrDefault(
//                    q => q.QuestionId == questionVM.QuestionId);

//                if (existingQuestion == null)
//                {
//                    // New question added to the exam
//                    var newExamQuestion = new ExamQuestions(
//                        questionVM.QuestionId,
//                        exam.Id,
//                        questionVM.ActualMark,
//                        questionVM.BonusMark,
//                        0 // NegativeMark for now
//                    );

//                    await _examQuestionsRepository.Create(newExamQuestion);
//                }
//                else
//                {
//                    // Existing question: update its marks
//                    existingQuestion.Update(
//                        questionVM.ActualMark,
//                        questionVM.BonusMark,
//                        existingQuestion.NegativeMark
//                    );

//                    _examQuestionsRepository.Update(existingQuestion);
//                }
//            }

//            // =========================================
//            // Recalculate Total Marks
//            // =========================================

//            double totalMarks = selectedQuestions
//                .Sum(q => q.ActualMark);

//            // Update the Exam itself
//            exam.Update(
//                model.Title,
//                model.Duration,
//                totalMarks,
//                model.TopicId,
//                model.StartDate,
//                model.EndDate
//            );

//            _examRepository.Update(exam);

//            // Save changes
//            await _examQuestionsRepository.SaveAsync();
//            await _examRepository.SaveAsync();

//            return true;
//        }

//        // =========================
//        // Delete Exam
//        // =========================

//        public async Task<bool> DeleteExamAsync(int id)
//        {
//            var exam = await _examRepository.GetBy(
//                e => e.Id == id);

//            if (exam == null)
//                return false;

//            _examRepository.Delete(exam);

//            await _examRepository.SaveAsync();

//            return true;
//        }
//    }
//}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using QuizEra.BLL.ModelVM.Exam;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
    public class ExamService : IExamService
    {
        private readonly IGenericRepository<Exam> _examRepository;
        private readonly IGenericRepository<ExamQuestions> _examQuestionsRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IGenericRepository<Topic> _topicRepository;
        private readonly IGenericRepository<ExamTopic> _examTopicRepository;
        private readonly IGenericRepository<Course> _courseRepository;
        private readonly INotificationService _notificationService;

        public ExamService(
            IGenericRepository<Exam> examRepository,
            IGenericRepository<ExamQuestions> examQuestionsRepository,
            IGenericRepository<Question> questionRepository,
            IGenericRepository<Topic> topicRepository,
            IGenericRepository<ExamTopic> examTopicRepository,
            IGenericRepository<Course> courseRepository,
            INotificationService notificationService)
        {
            _examRepository = examRepository;
            _examQuestionsRepository = examQuestionsRepository;
            _questionRepository = questionRepository;
            _topicRepository = topicRepository;
            _examTopicRepository = examTopicRepository;
            _courseRepository = courseRepository;
            _notificationService = notificationService;
        }

        // =========================
        // Create Exam
        // =========================

        public async Task<bool> CreateExamAsync(CreateExamVM model)
        {
            // Validate dates
            if (model.StartDate >= model.EndDate)
                return false;

            // Must select at least one topic
            if (model.TopicIds == null || !model.TopicIds.Any())
                return false;

            // Get selected topics
            var topics = (await _topicRepository.Get(
                filter: t => model.TopicIds.Contains(t.Id)))
                .ToList();

            // All selected topics must exist
            if (topics.Count != model.TopicIds.Distinct().Count())
                return false;

            // IMPORTANT:
            // All selected topics must belong to the selected course
            if (topics.Any(t => t.CourseID != model.CourseId))
                return false;

            // Get selected questions
            var selectedQuestions = model.Questions
                .Where(q => q.IsSelected)
                .ToList();

            // Exam must contain at least one question
            if (!selectedQuestions.Any())
                return false;

            // Validate questions
            foreach (var questionVM in selectedQuestions)
            {
                var question = await _questionRepository.GetBy(
                    q => q.Id == questionVM.QuestionId);

                if (question == null)
                    return false;

                // Question must belong to one of the selected topics
                if (!model.TopicIds.Contains(question.TopicID))
                    return false;

                if (questionVM.ActualMark <= 0)
                    return false;

                if (questionVM.BonusMark < 0)
                    return false;
            }

            // Calculate total marks
            double totalMarks = selectedQuestions.Sum(q => q.ActualMark);



            Console.WriteLine("CREATE EXAM VALIDATION PASSED");
            Console.WriteLine($"CourseId: {model.CourseId}");
            Console.WriteLine($"TopicIds: {string.Join(",", model.TopicIds)}");
            Console.WriteLine($"Selected Questions: {selectedQuestions.Count}");
            Console.WriteLine($"Total Marks: {totalMarks}");
            var exam = new Exam(
                model.Title,
                model.Duration,
                totalMarks,
                model.StartDate,
                model.EndDate
            );

            await _examRepository.Create(exam);
            await _examRepository.SaveAsync();

            // Create ExamTopics
            foreach (var topicId in model.TopicIds.Distinct())
            {
                var examTopic = new ExamTopic(
                    exam.Id,
                    topicId
                );

                await _examTopicRepository.Create(examTopic);
            }

            await _examTopicRepository.SaveAsync();

            // Create ExamQuestions
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
            var course = await _courseRepository.GetBy(c => c.Id == model.CourseId);
            string courseName = course?.CourseName ?? "Course";

            await _notificationService.CreateAndBroadcastExamNotificationAsync(
                courseId: model.CourseId,
                courseName: courseName,
                examTitle: exam.Title,
                examId: exam.Id
            );
            return true;
        }

        // =========================
        // Get All Exams
        // =========================

        public async Task<IEnumerable<ExamVM>> GetAllExamsAsync()
        {
            var exams = await _examRepository.Get();

            var examTopics = await _examTopicRepository.Get();

            // 1. جلب كل الأسئلة المرتبطة بالامتحانات
            var examQuestions = await _examQuestionsRepository.Get();

            var topics = await _topicRepository.Get(
                includeProperties: new List<Expression<Func<Topic, object>>>
                {
            t => t.Course
                });

            var result = exams.Select(exam =>
            {
                var selectedTopics = examTopics
                    .Where(et => et.ExamId == exam.Id)
                    .Select(et => topics.FirstOrDefault(t => t.Id == et.TopicId))
                    .Where(t => t != null)
                    .ToList();

                var firstTopic = selectedTopics.FirstOrDefault();

                // 2. حساب عدد الأسئلة لهذا الامتحان تحديداً
                int questionsCount = examQuestions.Count(eq => eq.ExamId == exam.Id);

                return new ExamVM
                {
                    Id = exam.Id,

                    CourseId = firstTopic?.CourseID ?? 0,

                    CourseName = firstTopic?.Course?.CourseName ?? string.Empty,

                    TopicIds = selectedTopics
                        .Select(t => t!.Id)
                        .ToList(),

                    TopicNames = selectedTopics
                        .Select(t => t!.Name)
                        .ToList(),

                    Title = exam.Title,
                    Duration = exam.Duration,
                    TotalMarks = exam.TotalMarks,
                    StartDate = exam.StartDate,
                    EndDate = exam.EndDate,

                    // 3. ربط النتيجة بالـ ViewModel
                    QuestionsCount = questionsCount
                };
            });

            return result;
        }

        // =========================
        // Get Exam By Id
        // =========================

        public async Task<ExamVM?> GetExamByIdAsync(int id)
        {
            var exam = await _examRepository.GetBy(
                e => e.Id == id);

            if (exam == null)
                return null;

            // Get ExamTopics
            var examTopics = await _examTopicRepository.Get(
                filter: et => et.ExamId == id);

            var topicIds = examTopics
                .Select(et => et.TopicId)
                .ToList();

            // Get topics
            var topics = await _topicRepository.Get(
     filter: t => topicIds.Contains(t.Id),
     includeProperties: new List<Expression<Func<Topic, object>>>
     {
        t => t.Course
     });
            var topicList = topics.ToList();

            var firstTopic = topicList.FirstOrDefault();

            var model = new ExamVM
            {
                Id = exam.Id,

                CourseId = firstTopic?.CourseID ?? 0,

                CourseName = firstTopic?.Course?.CourseName ?? string.Empty,

                TopicIds = topicList
                    .Select(t => t.Id)
                    .ToList(),

                TopicNames = topicList
                    .Select(t => t.Name)
                    .ToList(),

                Title = exam.Title,
                Duration = exam.Duration,
                TotalMarks = exam.TotalMarks,
                StartDate = exam.StartDate,
                EndDate = exam.EndDate
            };

            // Get exam questions
            var examQuestions = await _examQuestionsRepository.Get(
                filter: eq => eq.ExamId == id);

            foreach (var examQuestion in examQuestions)
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

            // Must select at least one topic
            if (model.TopicIds == null || !model.TopicIds.Any())
                return false;

            // Get exam
            var exam = await _examRepository.GetBy(
                e => e.Id == model.Id);

            if (exam == null)
                return false;

            // Get selected topics
            var topics = (await _topicRepository.Get(
                filter: t => model.TopicIds.Contains(t.Id)))
                .ToList();

            // Validate all topics
            if (topics.Count != model.TopicIds.Distinct().Count())
                return false;

            // All topics must belong to the same course
            if (topics.Any(t => t.CourseID != model.CourseId))
                return false;

            // Get selected questions
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

                // Question must belong to selected topic
                if (!model.TopicIds.Contains(question.TopicID))
                    return false;

                if (questionVM.ActualMark <= 0)
                    return false;

                if (questionVM.BonusMark < 0)
                    return false;
            }

            // =========================
            // Update ExamTopics
            // =========================

            var existingExamTopics = (await _examTopicRepository.Get(
                filter: et => et.ExamId == model.Id))
                .ToList();

            var selectedTopicIds = model.TopicIds
                .Distinct()
                .ToList();

            // -----------------------------------------
            // Remove topics that are no longer selected
            // -----------------------------------------

            foreach (var existingExamTopic in existingExamTopics)
            {
                if (!selectedTopicIds.Contains(existingExamTopic.TopicId))
                {
                    _examTopicRepository.HardDelete(existingExamTopic);
                }
            }
            // SAVE THE DELETIONS FIRST
            await _examTopicRepository.SaveAsync();
            // -----------------------------------------
            // Add only newly selected topics
            // -----------------------------------------

            foreach (var topicId in selectedTopicIds)
            {
                bool alreadyExists = existingExamTopics
                    .Any(et => et.TopicId == topicId);

                if (!alreadyExists)
                {
                    var newExamTopic = new ExamTopic(
                        model.Id,
                        topicId
                    );

                    await _examTopicRepository.Create(newExamTopic);
                }
            }

            await _examTopicRepository.SaveAsync();

            // =========================
            // Update ExamQuestions
            // =========================

            var existingQuestions = (await _examQuestionsRepository.Get(
                filter: eq => eq.ExamId == model.Id))
                .ToList();

            // Remove questions that are no longer selected
            foreach (var existingQuestion in existingQuestions)
            {
                bool stillSelected = selectedQuestions.Any(
                    q => q.QuestionId == existingQuestion.QuestionId);

                if (!stillSelected)
                {
                    _examQuestionsRepository.HardDelete(existingQuestion);
                }
            }

            // Add / update selected questions
            foreach (var questionVM in selectedQuestions)
            {
                var existingQuestion = existingQuestions.FirstOrDefault(
                    q => q.QuestionId == questionVM.QuestionId);

                if (existingQuestion == null)
                {
                    var newExamQuestion = new ExamQuestions(
                        questionVM.QuestionId,
                        model.Id,
                        questionVM.ActualMark,
                        questionVM.BonusMark,
                        0
                    );

                    await _examQuestionsRepository.Create(newExamQuestion);
                }
                else
                {
                    existingQuestion.Update(
                        questionVM.ActualMark,
                        questionVM.BonusMark,
                        existingQuestion.NegativeMark
                    );

                    _examQuestionsRepository.Update(existingQuestion);
                }
            }

            // =========================
            // Recalculate Total Marks
            // =========================

            double totalMarks = selectedQuestions
                .Sum(q => q.ActualMark);

            // =========================
            // Update Exam
            // =========================

            exam.Update(
                model.Title,
                model.Duration,
                totalMarks,
                model.StartDate,
                model.EndDate
            );

            _examRepository.Update(exam);

            // Save
            await _examQuestionsRepository.SaveAsync();
            await _examRepository.SaveAsync();

            return true;
        }

        //DEtails//////

        public async Task<ExamVM?> GetExamDetailsAsync(int id)
        {
            // Get the exam
            var exam = await _examRepository.GetBy(
                e => e.Id == id
            );

            if (exam == null)
                return null;


            // Get Exam Topics
            var examTopics = await _examTopicRepository.Get(
                filter: et => et.ExamId == id
            );


            // Get Topics with Course
            var topics = await _topicRepository.Get(
                includeProperties: new List<Expression<Func<Topic, object>>>
                {
            t => t.Course
                }
            );


            // Get Exam Questions
            var examQuestions = await _examQuestionsRepository.Get(
                filter: eq => eq.ExamId == id
            );


            // Build selected topics
            var selectedTopics = examTopics
                .Select(et => topics.FirstOrDefault(t => t.Id == et.TopicId))
                .Where(t => t != null)
                .ToList();


            var firstTopic = selectedTopics.FirstOrDefault();


            // Build question list
            var questionIds = examQuestions
                .Select(eq => eq.QuestionId)
                .ToList();


            var questions = await _questionRepository.Get(
                filter: q => questionIds.Contains(q.Id)
            );


            return new ExamVM
            {
                Id = exam.Id,

                CourseId = firstTopic?.CourseID ?? 0,

                CourseName = firstTopic?.Course?.CourseName ?? string.Empty,

                TopicIds = selectedTopics
                    .Select(t => t!.Id)
                    .ToList(),

                TopicNames = selectedTopics
                    .Select(t => t!.Name)
                    .ToList(),

                Title = exam.Title,

                Duration = exam.Duration,

                TotalMarks = exam.TotalMarks,

                StartDate = exam.StartDate,

                EndDate = exam.EndDate,

                Questions = examQuestions
                    .Select(eq =>
                    {
                        var question = questions
                            .FirstOrDefault(q => q.Id == eq.QuestionId);

                        return new CreateExamQuestionVM
                        {
                            QuestionId = eq.QuestionId,

                            QuestionText = question?.QuestionText ?? string.Empty,

                            ActualMark = eq.ActualMark,

                            BonusMark = eq.BonusMark,

                            IsSelected = true
                        };
                    })
                    .ToList()
            };
        }

        // =========================
        // Delete Exam
        // =========================

        public async Task<bool> DeleteExamAsync(int id)
        {

            // Get Exam

            var exam = await _examRepository.GetBy(
                e => e.Id == id);

            if (exam == null)
                return false;

            // Delete ExamTopics


            var examTopics = (await _examTopicRepository.Get(
                filter: et => et.ExamId == id))
                .ToList();

            foreach (var examTopic in examTopics)
            {
                _examTopicRepository.HardDelete(examTopic);
            }

            await _examTopicRepository.SaveAsync();

            // Delete ExamQuestions

            var examQuestions = (await _examQuestionsRepository.Get(
                filter: eq => eq.ExamId == id))
                .ToList();

            foreach (var examQuestion in examQuestions)
            {
                _examQuestionsRepository.HardDelete(examQuestion);
            }

            await _examQuestionsRepository.SaveAsync();

            // Delete Exam

            _examRepository.HardDelete(exam);

            await _examRepository.SaveAsync();
            var checkExam = await _examRepository.GetBy(
    e => e.Id == id);

            return true;
        }
    }
}