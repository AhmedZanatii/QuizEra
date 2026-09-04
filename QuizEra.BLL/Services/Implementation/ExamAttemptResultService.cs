using QuizEra.BLL.ModelVM.ExamAttempt;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
    public class ExamAttemptResultService : IExamAttemptResultService
    {
        private readonly IGenericRepository<StudentExamAttempt> _attemptRepository;
        private readonly IGenericRepository<ExamQuestions> _examQuestionRepository;
        private readonly IGenericRepository<Complaint> _complaintRepository;

        public ExamAttemptResultService(
            IGenericRepository<StudentExamAttempt> attemptRepository,
            IGenericRepository<ExamQuestions> examQuestionRepository,
            IGenericRepository<Complaint> complaintRepository)
        {
            _attemptRepository = attemptRepository;
            _examQuestionRepository = examQuestionRepository;
            _complaintRepository = complaintRepository;
        }

        public async Task<ExamAttemptResultVM> GetAttemptResultAsync(int attemptId)
        {
            var attempt = await _attemptRepository.GetBy(
                a => a.Id == attemptId && !a.IsDeleted,
                [
                    a => a.Exam,
                    a => a.Student,
                    a => a.Student.AppUser,
                    a => a.StudentExamQuestionAnswers
                ],
                noTrack: true
            );

            if (attempt == null)
                throw new Exception($"Attempt {attemptId} was not found.");

            var result = new ExamAttemptResultVM
            {
                AttemptId = attempt.Id,
                ExamId = attempt.ExamId,
                ExamTitle = attempt.Exam?.Title ?? string.Empty,
                TotalScore = attempt.StudResult,
                CompletionTime = attempt.EndTime.HasValue ? attempt.EndTime.Value - attempt.StartTime : null,
                CorrectAnswersCount = 0,
                IncorrectAnswersCount = 0,
                QuestionBreakdown = new List<QuestionResultDetailVM>()
            };

            if (attempt.Exam != null && attempt.Exam.TotalMarks > 0)
            {
                result.Percentage = ((decimal)attempt.StudResult / (decimal)attempt.Exam.TotalMarks) * 100;
                result.IsPassed = result.Percentage >= 50;
            }
            else
            {
                result.Percentage = 0;
                result.IsPassed = false;
            }

            // Pre-fetch complaints for this attempt to prevent N+1 database queries inside the loop
            var attemptComplaints = await _complaintRepository.Get(
                c => c.ExamAttemptId == attempt.Id && !c.IsDeleted,
                noTrack: true
            );

            // Store question IDs with complaints in a HashSet for O(1) memory lookup
            var questionsWithComplaints = attemptComplaints
                .Select(c => c.ExamQuestionId)
                .ToHashSet();

            var questionAnswers = attempt.StudentExamQuestionAnswers
                .OrderBy(a => a.ExamQuestionsId)
                .ToList();

            var questionNumber = 1;

            foreach (var answer in questionAnswers)
            {
                var examQuestion = await _examQuestionRepository.GetBy(
                    eq => eq.Id == answer.ExamQuestionsId,
                    [eq => eq.Question.Options],
                    noTrack: true);

                if (examQuestion == null || examQuestion.Question == null)
                    continue;

                var question = examQuestion.Question;
                var isCorrect = answer.IsCorrect;

                if (isCorrect)
                    result.CorrectAnswersCount++;
                else
                    result.IncorrectAnswersCount++;

                var questionDetail = new QuestionResultDetailVM
                {
                    QuestionId = question.Id,
                    ExamQuestionId = answer.ExamQuestionsId,
                    QuestionNumber = questionNumber,
                    QuestionText = question.QuestionText,
                    QuestionFormat = question.QuestionFormat.ToString(),
                    CorrectAnswer = question.QuestionAnswer ?? string.Empty,
                    StudentAnswer = answer.QuestionAnswer ?? string.Empty,
                    QuestionMark = answer.StudQMarks,
                    AIJustification = answer.AIJustification,
                    IsCorrect = isCorrect,
                    TimeSpent = answer.TimeSpent,
                    HasComplaint = questionsWithComplaints.Contains(answer.ExamQuestionsId),
                    Options = question.Options?.Select(o => new OptionResultVM
                    {
                        OptionText = o.OptionText,
                        IsCorrect = o.IsCorrect
                    }).ToList() ?? new()
                };

                result.QuestionBreakdown.Add(questionDetail);

                questionNumber++;
            }

            return result;
        }
    }
}