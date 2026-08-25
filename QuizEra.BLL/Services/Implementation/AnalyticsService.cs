using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;
using QuizEra.BLL.ModelVM.Analytics;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IGenericRepository<StudentExamAttempt> _attemptRepo;
        private readonly IGenericRepository<Exam> _examRepo;

        public AnalyticsService(
            IGenericRepository<StudentExamAttempt> attemptRepo,
            IGenericRepository<Exam> examRepo)
        {
            _attemptRepo = attemptRepo;
            _examRepo = examRepo;
        }

        public async Task<StudentAnalyticsVM> GetStudentAnalyticsAsync(int studentExamAttemptId)
        {
            var includes = new List<Expression<Func<StudentExamAttempt, object>>>
            {
                a => a.Exam,
                a => a.Student,
                a => a.Student.AppUser,
                a => a.StudentExamQuestionAnswers
            };

            var attempts = await _attemptRepo.Get(
                filter: a => a.Id == studentExamAttemptId,
                includeProperties: includes
            );

            var attempt = attempts.FirstOrDefault();
            if (attempt == null) return null;

            var maxScore = attempt.Exam?.TotalMarks ?? 0;
            var score = attempt.StudResult;
            var percentage = maxScore > 0 ? ((decimal)score / (decimal)maxScore) * 100 : 0;

            bool isPassed = percentage >= 50; 

            var vm = new StudentAnalyticsVM
            {
                AttemptId = attempt.Id,
                StudentName = attempt.Student?.AppUser?.UserName ?? "Student",
                ExamTitle = attempt.Exam?.Title,
                TotalScore = score,
                Percentage = percentage,
                IsPassed = isPassed
            };

            if (attempt.EndTime.HasValue)
            {
                vm.CompletionTime = attempt.EndTime.Value - attempt.StartTime;
            }

            if (attempt.StudentExamQuestionAnswers != null)
            {
                foreach (var ans in attempt.StudentExamQuestionAnswers)
                {
                    // Basic exact string matching for correct answer
                    bool correct = ans.QuestionAnswer == ans.ExamQuestions?.Question?.QuestionAnswer;

                    if (correct)
                        vm.CorrectAnswersCount++;
                    else
                        vm.IncorrectAnswersCount++;

                    vm.QuestionBreakdown.Add(new QuestionAnalyticsVM
                    {
                        QuestionText = ans.ExamQuestions?.Question?.QuestionText,
                        IsCorrect = correct,
                        TimeSpent = ans.TimeSpent
                    });
                }
            }

            return vm;
        }

        public async Task<ClassAnalyticsVM> GetClassAnalyticsAsync(int examId)
        {
            var includes = new List<Expression<Func<StudentExamAttempt, object>>>
            {
                a => a.Student,
                a => a.Student.AppUser,
                a => a.StudentExamQuestionAnswers
            };

            var attempts = await _attemptRepo.Get(
                filter: a => a.ExamId == examId,
                includeProperties: includes
            );

            var examResult = await _examRepo.Get(filter: e => e.Id == examId);
            var exam = examResult.FirstOrDefault();

            if (!attempts.Any() || exam == null) return new ClassAnalyticsVM();

            var orderedAttempts = attempts.OrderByDescending(a => a.StudResult).ToList();

            var classAnalytics = new ClassAnalyticsVM
            {
                ExamId = examId,
                ExamTitle = exam.Title,
                HighestScore = orderedAttempts.First().StudResult,
                LowestScore = orderedAttempts.Last().StudResult,
                AverageScore = orderedAttempts.Average(a => a.StudResult)
            };

            int rank = 1;
            foreach (var att in orderedAttempts)
            {
                classAnalytics.StudentRankings.Add(new StudentRankingVM
                {
                    StudentName = att.Student?.AppUser?.UserName ?? "Student",
                    Score = att.StudResult,
                    Rank = rank++
                });

                // Simple pass/fail distribution logic
                string gradeKey = att.StudResult >= (exam.TotalMarks / 2.0) ? "Pass" : "Fail";
                if (!classAnalytics.GradeDistribution.ContainsKey(gradeKey))
                {
                    classAnalytics.GradeDistribution[gradeKey] = 0;
                }
                classAnalytics.GradeDistribution[gradeKey]++;
            }

            // Frequently missed questions
            var allAnswers = attempts.SelectMany(a => a.StudentExamQuestionAnswers).ToList();
            var incorrectAnswers = allAnswers.Where(ans => ans.QuestionAnswer != ans.ExamQuestions?.Question?.QuestionAnswer);

            var groupedMisses = incorrectAnswers
                .GroupBy(a => a.ExamQuestions?.Question?.QuestionText)
                .Select(g => new MissedQuestionVM
                {
                    QuestionText = g.Key,
                    MissCount = g.Count()
                })
                .OrderByDescending(m => m.MissCount)
                .Take(5)
                .ToList();

            classAnalytics.FrequentlyMissedQuestions = groupedMisses;

            return classAnalytics;
        }

        public Task<byte[]> ExportClassReportAsync(int examId, string format)
        {
            throw new NotImplementedException("Exporting will be implemented in the Reporting task.");
        }
    }
}