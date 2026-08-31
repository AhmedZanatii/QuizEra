using QuizEra.BLL.ModelVM.StudentExamAttempt;
using QuizEra.BLL.ModelVM.StudentExamQuestionAnswer;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Repositories.Abstraction;
using QuizEra.DAL.Entities;
using Microsoft.IdentityModel.Tokens;

namespace QuizEra.BLL.Services.Implementation
{
    public class StudentExamAttemptService : IStudentExamAttemptService
    {
        private IGenericRepository<StudentExamAttempt> AttemptRepo;
        private IGenericRepository<Student> StudentRepo;
        public StudentExamAttemptService(IGenericRepository<StudentExamAttempt> AttemptRepo, IGenericRepository<Student> studentRepo)
        {
            this.AttemptRepo = AttemptRepo;
            this.StudentRepo = studentRepo;
        }

        public async Task AddAnswerAsync(StudentExamAttemptVM attempt, StudentExamQuestionAnswerVM answer, string user)
        {
            if(attempt == null || answer == null || answer.QuestionAnswer.IsNullOrEmpty()) 
            {
                throw new ArgumentException("Invalid StudentExamQuestionAnswerVM provided for addition.");
            }

            var existingAttempt = (await AttemptRepo.Get(a => a.ExamId == attempt.ExamId && a.StudentId == attempt.StudentId && !a.IsDeleted, [a => a.StudentExamQuestionAnswers])).FirstOrDefault();

            if(existingAttempt == null) 
            {
                throw new ArgumentException("Attempt doesn't exist in the database.");
            }

            var newAnswer = new StudentExamQuestionAnswer(
                answer.ExamQuestionId,
                answer.StudentExamAttemptId,
                answer.StudQMarks,
                answer.QuestionAnswer,
                user,
                answer.IsCorrect,
                answer.TimeSpent,
                null);

            existingAttempt.AnswerQuestion(newAnswer, user);
            await AttemptRepo.SaveAsync();
        }

        public async Task<IEnumerable<StudentExamAttemptVM>> GetByExamIdAsync(int id)
        {
            var attempts = (await AttemptRepo.Get(a => a.ExamId == id && !a.IsDeleted, [a => a.StudentExamQuestionAnswers])).ToList();

            if(attempts == null || !attempts.Any())
            {
                throw new Exception($"No Exam Attempt found for ExamId {id}");
            }

            return attempts.Select(MapToViewModel);
        }

        public async Task<IEnumerable<StudentExamAttemptVM>> GetByStudentIdAsync(string id)
        {
            var student = await StudentRepo.GetBy(
                filter: s => s.AppUserId == id,
                noTrack: true
            );

            if (student == null)
            {
                throw new Exception($"No Student found for StudentId {id}");
            }

            var attempts = (await AttemptRepo.Get(a => a.StudentId == student.Id && !a.IsDeleted, [a => a.StudentExamQuestionAnswers])).ToList();

            if(attempts == null || !attempts.Any())
            {
                throw new Exception($"No Exam Attempt found for StudentId {student.Id}");
            }

            return attempts.Select(MapToViewModel);
        }

        public async Task<StudentExamAttemptVM> GetExactAttemptAsync(int examId, string studentId)
        {
            var student = await StudentRepo.GetBy(
                filter: s => s.AppUserId == studentId,
                noTrack: true
            );

            if (student == null)
            {
                throw new Exception($"No Student found for StudentId {studentId}");
            }

            var attempt = (await AttemptRepo.Get(a => a.ExamId == examId && a.StudentId == student.Id && !a.IsDeleted, [a => a.StudentExamQuestionAnswers])).FirstOrDefault();

            if(attempt == null)
            {
                throw new Exception($"No Exam Attempt found for ExamId {examId} and StudentId {studentId}");
            }

            return MapToViewModel(attempt);
        }

        public async Task<StudentExamAttemptVM> StartAttemptAsync(int examId, string studentUserId, string creatorUser)
        {
            var student = await StudentRepo.GetBy(s => s.AppUserId == studentUserId, noTrack: true);
            if (student == null)
                throw new Exception($"No Student found for StudentId {studentUserId}");

            var existingAttempt = (await AttemptRepo.Get(
                a => a.ExamId == examId && a.StudentId == student.Id && !a.IsDeleted,
                [a => a.StudentExamQuestionAnswers])).FirstOrDefault();

            if (existingAttempt != null)
            {
                if (existingAttempt.ShuffleSeed == 0)
                {
                    existingAttempt.SetShuffleSeed(new Random().Next(1, int.MaxValue));
                    AttemptRepo.Update(existingAttempt);
                    await AttemptRepo.SaveAsync();
                }

                return MapToViewModel(existingAttempt);
            }

            var shuffleSeed = new Random().Next(1, int.MaxValue);
            await AttemptRepo.Create(new StudentExamAttempt(
                examId, student.Id, 0, DateTime.UtcNow, creatorUser, shuffleSeed));
            await AttemptRepo.SaveAsync();

            return await GetExactAttemptAsync(examId, studentUserId);
        }

        public async Task CompleteAttemptAsync(int examId, string studentUserId, string modifierUser)
        {
            var student = await StudentRepo.GetBy(s => s.AppUserId == studentUserId, noTrack: true);
            if (student == null)
                throw new Exception($"No Student found for StudentId {studentUserId}");

            var attempt = (await AttemptRepo.Get(
                a => a.ExamId == examId && a.StudentId == student.Id && !a.IsDeleted)).FirstOrDefault();
            if (attempt == null)
                throw new Exception($"StudentExamAttempt not found for ExamId {examId} and StudentId {studentUserId}");

            attempt.EndAttempt(DateTime.UtcNow);
            AttemptRepo.Update(attempt);
            await AttemptRepo.SaveAsync();
        }
        
        public async Task AddAsync(StudentExamAttemptVM attempt, string creatorUser)
        {
            if(attempt == null) 
            {
                throw new ArgumentException("Invalid StudentExamAttemptVM provided for addition.");
            }

            await AttemptRepo.Create(new StudentExamAttempt(attempt.ExamId, attempt.StudentId, attempt.StudResult, attempt.StartTime, creatorUser));
            await AttemptRepo.SaveAsync();
        }

        public async Task UpdateAsync(StudentExamAttemptVM attempt, string modifierUser)
        {
            if(attempt == null) 
            {
                throw new ArgumentException("Invalid StudentExamAttempt provided for update.");
            }

            // Fixed: Included [a => a.StudentExamQuestionAnswers] here so EF tracks child answers during updates
            var existingAttempt = (await AttemptRepo.Get(a => a.StudentId == attempt.StudentId && a.ExamId == attempt.ExamId && !a.IsDeleted, [a => a.StudentExamQuestionAnswers])).FirstOrDefault();
            
            if (existingAttempt == null)
            {
                throw new Exception($"StudentExamAttempt not found for ExamId {attempt.ExamId} and StudentId {attempt.StudentId}");
            }

            existingAttempt.UpdateResult(attempt.StudResult, modifierUser);
            AttemptRepo.Update(existingAttempt);
            await AttemptRepo.SaveAsync();
        }

        public async Task DeleteAsync(int examId, string studentId, string deleterUser)
        {
            var student = await StudentRepo.GetBy(
                filter: s => s.AppUserId == studentId,
                noTrack: true
            );

            if (student == null)
            {
                throw new Exception($"No Student found for StudentId {studentId}");
            }

            var existingAttempt = (await AttemptRepo.Get(a => a.ExamId == examId && a.StudentId == student.Id && !a.IsDeleted)).FirstOrDefault();
            
            if (existingAttempt == null)
            {
                throw new Exception($"StudentExamAttempt not found or already deleted for ExamId {examId} and StudentId {studentId}");
            }

            existingAttempt.Delete(deleterUser, DateTime.UtcNow);
            AttemptRepo.Delete(existingAttempt);
            await AttemptRepo.SaveAsync();
        }

        // Minimal helper to map entity collection to VM collection securely
        private static StudentExamAttemptVM MapToViewModel(StudentExamAttempt a)
        {
            return new StudentExamAttemptVM
            {
                AttemptId = a.Id,
                ExamId = a.ExamId,
                StudentId = a.StudentId,
                StudResult = a.StudResult,
                StartTime = a.StartTime,
                EndTime = a.EndTime,
                ShuffleSeed = a.ShuffleSeed,
                StudentExamQuestionAnswers = a.StudentExamQuestionAnswers.Select(ans => new StudentExamQuestionAnswerVM
                {
                    ExamQuestionId = ans.ExamQuestionsId,
                    StudentExamAttemptId = ans.StudentExamAttemptId,
                    StudQMarks = ans.StudQMarks,
                    QuestionAnswer = ans.QuestionAnswer,
                    TimeSpent = ans.TimeSpent
                }).ToList()
            };
        }
    }
}