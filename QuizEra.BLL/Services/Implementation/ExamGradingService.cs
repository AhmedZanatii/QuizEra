using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Entities.Enums;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services.Implementation
{
    public class ExamGradingService : IExamGradingService
    {
        private readonly IGenericRepository<StudentExamAttempt> _attemptRepository;
        private readonly IGenericRepository<ExamQuestions> _examQuestionRepository;
        private readonly IGenericRepository<Question> _questionRepository;
        private readonly IAIEvaluationService _aiService;

        public ExamGradingService(
            IGenericRepository<StudentExamAttempt> attemptRepository,
            IGenericRepository<ExamQuestions> examQuestionRepository,
            IGenericRepository<Question> questionRepository,
            IAIEvaluationService aiService)
        {
            _attemptRepository = attemptRepository;
            _examQuestionRepository = examQuestionRepository;
            _questionRepository = questionRepository;
            _aiService = aiService;
        }

        public async Task GradeAttemptAsync(int attemptId, string modifierUser)
        {
            var attempt = await _attemptRepository.GetBy(
                a => a.Id == attemptId && !a.IsDeleted,
                [a => a.StudentExamQuestionAnswers]);

            if (attempt == null)
                throw new ArgumentException($"Attempt {attemptId} was not found.");

            var totalScore = 0;
            
            foreach (var answer in attempt.StudentExamQuestionAnswers)
            {
                var examQuestion = await _examQuestionRepository.GetBy(
                    eq => eq.Id == answer.ExamQuestionsId,
                    noTrack: true);
                if (examQuestion == null)
                    continue;

                var question = await _questionRepository.GetBy(
                    q => q.Id == examQuestion.QuestionId,
                    [q => q.Options], // Ensure Options are included for grading
                    noTrack: true);
                if (question == null)
                    continue;

                int marks = 0;
                bool isCorrect = false;
                int maxMarks = Convert.ToInt32(Math.Round(examQuestion.ActualMark));

                if (question.QuestionFormat == QuestionFormat.MCQ || question.QuestionFormat == QuestionFormat.TrueFalse)
                {
                    // Find the single correct option for this question
                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                    
                    // Compare the selected text with the correct option text
                    if (correctOption != null && !string.IsNullOrWhiteSpace(answer.QuestionAnswer) &&
                        correctOption.OptionText.Trim().Equals(answer.QuestionAnswer.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        isCorrect = true;
                        marks = maxMarks;
                    }
                    
                    // Update the answer entity with the results
                    answer.Update(marks, answer.QuestionAnswer, modifierUser, isCorrect, answer.TimeSpent);
                    totalScore += marks;
                }
                else if (question.QuestionFormat == QuestionFormat.Essay)
                {
                    var aiResult = await _aiService.EvaluateEssayAsync(
                        question.QuestionText, 
                        question.QuestionAnswer ?? "", 
                        answer.QuestionAnswer, 
                        maxMarks);
                        
                    answer.Update(aiResult.Grade, answer.QuestionAnswer, modifierUser, aiResult.IsCorrect, answer.TimeSpent, aiResult.Justification);
                    totalScore += aiResult.Grade;
                }
            }

            attempt.UpdateResult(totalScore, modifierUser);
            _attemptRepository.Update(attempt);
            await _attemptRepository.SaveAsync();
        }
    }
}