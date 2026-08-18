using QuizEra.BLL.ModelVM.StudentExamQuestionAnswer;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Repositories.Abstraction;
using QuizEra.DAL.Entities;

namespace QuizEra.BLL.Services.Implementation
{
    public class StudentExamQuestionAnswerService : IStudentQuestionAnswerService
    {
        private IGenericRepository<StudentExamQuestionAnswer> Repo;
        StudentExamQuestionAnswerService(IGenericRepository<StudentExamQuestionAnswer> AnswerRepo)
        {
            Repo = AnswerRepo;
        }

        public async Task<IEnumerable<StudentExamQuestionAnswerVM>> GetByExamAttemptIdAsync(int id)
        {
            try
            {
                // Retrieve all StudentExamQuestionAnswers for the given ExamAttemptId
                var answers = await Repo.Get(a => a.StudentExamAttemptId == id);

                // Check if any answers were found
                if(answers == null || !answers.Any())
                {
                    throw new Exception($"No StudentExamQuestionAnswers found for ExamAttemptId {id}");
                }

                // Map the retrieved answers to StudentExamQuestionAnswerVM (Change to automapper later)
                return answers.Select(a => new StudentExamQuestionAnswerVM
                {
                    StudentExamAttemptId = a.StudentExamAttemptId,
                    ExamQuestionId = a.ExamQuestionsId,
                    QuestionAnswer = a.QuestionAnswer
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving StudentExamQuestionAnswer with ID {id}: {ex.Message}", ex);
            }
        }
        public async Task<IEnumerable<StudentExamQuestionAnswerVM>> GetByExamQuestionIdAsync(int id)
        {
            try
            {
                // Retrieve all StudentExamQuestionAnswers for the given ExamQuestionId
                var answers = await Repo.Get(a => a.ExamQuestionsId == id);

                // Check if any answers were found
                if(answers == null || !answers.Any())
                {
                    throw new Exception($"No StudentExamQuestionAnswers found for ExamQuestionId {id}");
                }

                // Map the retrieved answers to StudentExamQuestionAnswerVM (Change to automapper later)
                return answers.Select(a => new StudentExamQuestionAnswerVM
                {
                    StudentExamAttemptId = a.StudentExamAttemptId,
                    ExamQuestionId = a.ExamQuestionsId,
                    QuestionAnswer = a.QuestionAnswer
                });
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving all StudentExamQuestionAnswers: {ex.Message}", ex);
            }
        }
        public async Task AddAsync(StudentExamQuestionAnswerVM answer)
        {
            try
            {
                // Validate the input StudentExamQuestionAnswerVM
                if(answer == null || string.IsNullOrWhiteSpace(answer.QuestionAnswer) || answer.StudentExamAttemptId <= 0 || answer.ExamQuestionId <= 0) 
                {
                    throw new ArgumentException("Invalid StudentExamQuestionAnswerVM provided for addition.");
                }

                // Create a new StudentExamQuestionAnswer entity and add it to the repository (Marks should be updated after grading)
                await Repo.Create(new StudentExamQuestionAnswer(answer.ExamQuestionId, answer.StudentExamAttemptId, 0, answer.QuestionAnswer));
                await Repo.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error adding StudentExamQuestionAnswer: {ex.Message}", ex);
            }
        }
        public async Task UpdateAsync(StudentExamQuestionAnswerVM answer)
        {
            try
            {
                // Validate the input StudentExamQuestionAnswerVM
                if(answer == null || string.IsNullOrWhiteSpace(answer.QuestionAnswer) || answer.StudentExamAttemptId <= 0 || answer.ExamQuestionId <= 0) 
                {
                    throw new ArgumentException("Invalid StudentExamQuestionAnswerVM provided for update.");
                }

                // Retrieve the existing StudentExamQuestionAnswer entity from the repository
                var existingAnswer = (await Repo.Get(a => a.StudentExamAttemptId == answer.StudentExamAttemptId && a.ExamQuestionsId == answer.ExamQuestionId)).FirstOrDefault();
                if (existingAnswer == null)
                {
                    throw new Exception($"StudentExamQuestionAnswer not found for ExamQuestionId {answer.ExamQuestionId} and ExamAttemptId {answer.StudentExamAttemptId}");
                }

                // Update the existing entity with the new values
                Repo.Update(new StudentExamQuestionAnswer(existingAnswer.ExamQuestionsId, existingAnswer.StudentExamAttemptId, existingAnswer.StudQMarks, answer.QuestionAnswer));
                await Repo.SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"Error updating StudentExamQuestionAnswer: {ex.Message}", ex);
            }
        }
    }
}