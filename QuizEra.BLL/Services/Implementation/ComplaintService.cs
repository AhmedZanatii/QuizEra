using QuizEra.BLL.ModelVM.Complaint;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;
using QuizEra.BLL.ModelVM.Exam;
using QuizEra.BLL.ModelVM.StudentExamAttempt;
using QuizEra.DAL.Entities.Enums;

namespace QuizEra.BLL.Services.Implementation
{
    public class ComplaintService : IComplaintService
    {
        private readonly IGenericRepository<Complaint> _complaintRepository;
        private readonly IGenericRepository<Student> _studentRepository;
        public ComplaintService(IGenericRepository<Complaint> complaintRepository, IGenericRepository<Student> studentRepository)
        {
            _complaintRepository = complaintRepository;
            _studentRepository = studentRepository;
        }
        public async Task<bool> CreateAsync(ComplaintVM complaintVM, string creatorUser)
        {
            if(complaintVM == null || string.IsNullOrWhiteSpace(complaintVM.Comment))
            {
                throw new ArgumentException("ComplaintVM or Comment cannot be null or whitespace.", nameof(complaintVM));
            }
            
            if(string.IsNullOrWhiteSpace(complaintVM.UserStudentId))
            {
                throw new ArgumentException("UserStudentId cannot be null or whitespace.", nameof(complaintVM.UserStudentId));
            }

            var student = await _studentRepository.GetBy(
                filter: s => s.AppUserId == complaintVM.UserStudentId,
                noTrack: true
            );

            if (student == null)
            {
                throw new Exception($"No Student found for AppUserId {complaintVM.UserStudentId}");
            }

            // Prevent duplicate complaints
            var existingComplaint = (await _complaintRepository.Get(
                c => c.ExamAttemptId == complaintVM.ExamAttemptId && c.ExamQuestionId == complaintVM.ExamQuestionId && !c.IsDeleted,
                noTrack: true)).FirstOrDefault();

            if (existingComplaint != null)
            {
                throw new InvalidOperationException("You have already submitted a complaint for this question.");
            }

            var complaint = new Complaint(
                complaintVM.ExamAttemptId,
                complaintVM.ExamQuestionId,
                complaintVM.Comment,
                creatorUser
            );

            await _complaintRepository.Create(complaint);
            await _complaintRepository.SaveAsync();
            return true;
        }

        public async Task<IEnumerable<ComplaintVM>> GetAllByExamIdAsync(int id)
        {
            var complaints = (await _complaintRepository.Get(
                c => c.ExamAttempt.ExamId == id,
                [a => a.ExamQuestion.Question, a => a.ExamAttempt.Student,
                a => a.ExamAttempt.Exam, a => a.ExamAttempt.StudentExamQuestionAnswers],
                noTrack: true
            )).Where(c => !c.IsDeleted).ToList();

            if(complaints == null || !complaints.Any())
            {
                return Enumerable.Empty<ComplaintVM>();
            }

            return complaints.Select(MapToVM);
        }

        public async Task<IEnumerable<ComplaintVM>> GetAllByStudentIdAsync(string id)
        {
            var student = await _studentRepository.GetBy(
                filter: s => s.AppUserId == id,
                noTrack: true
            );
            
            if (student == null)
            {
                return Enumerable.Empty<ComplaintVM>();
            }

            var complaints = (await _complaintRepository.Get(
                c => c.ExamAttempt.StudentId == student.Id,
                [a => a.ExamQuestion.Question, a => a.ExamAttempt.Student,
                a => a.ExamAttempt.Exam, a => a.ExamAttempt.StudentExamQuestionAnswers],
                noTrack: true
            )).Where(c => !c.IsDeleted).ToList();

            if(complaints == null || !complaints.Any())
            {
                return Enumerable.Empty<ComplaintVM>();
            }

            return complaints.Select(MapToVM);
        }

        public async Task<ComplaintVM?> GetByIdAsync(int id)
        {
            var complaint = (await _complaintRepository.Get(
                c => c.Id == id,
                [a => a.ExamQuestion.Question, a => a.ExamAttempt.Student,
                a => a.ExamAttempt.Exam, a => a.ExamAttempt.StudentExamQuestionAnswers],
                noTrack: true
            )).FirstOrDefault(c => !c.IsDeleted);

            if(complaint == null)
            {
                return null;
            }

            return MapToVM(complaint);
        }

        public async Task<bool> UpdateCommentAsync(int id, string comment, string modifierUser)
        {
            if(string.IsNullOrWhiteSpace(comment))
            {
                throw new ArgumentException("Comment cannot be null or whitespace.", nameof(comment));
            }

            var existingComplaint = (await _complaintRepository.Get(
                c => c.Id == id,
                noTrack: false
            )).FirstOrDefault(c => !c.IsDeleted);

            if(existingComplaint == null)
            {
                throw new Exception($"No Complaint found for Id {id}");
            }

            existingComplaint.UpdateComment(
                comment,
                modifierUser
            );

            _complaintRepository.Update(existingComplaint);
            await _complaintRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateResponseAsync(int id, string status, string response, string modifierUser)
        {
            var existingComplaint = (await _complaintRepository.Get(
                c => c.Id == id,
                noTrack: false
            )).FirstOrDefault(c => !c.IsDeleted);

            if(existingComplaint == null)
            {
                throw new Exception($"No Complaint found for Id {id}");
            }

            var complaintStatus = Enum.TryParse<ComplaintStatus>(status, out var parsedStatus) ? parsedStatus : throw new ArgumentException($"Invalid status value: {status}");

            existingComplaint.UpdateResponse(
                response,
                complaintStatus,
                modifierUser
            );

            _complaintRepository.Update(existingComplaint);
            await _complaintRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteExamAsync(int id, string deleterUser)
        {
            var existingComplaint = (await _complaintRepository.Get(
                c => c.Id == id,
                noTrack: false
            )).FirstOrDefault(c => !c.IsDeleted);

            if(existingComplaint == null)
            {
                throw new Exception($"No Complaint found for Id {id}");
            }

            existingComplaint.Delete(deleterUser, DateTime.UtcNow);

            _complaintRepository.Delete(existingComplaint);
            await _complaintRepository.SaveAsync();
            return true;
        }

        private static ComplaintVM MapToVM(Complaint complaint)
        {
            var studentAnswer = complaint.ExamAttempt?.StudentExamQuestionAnswers?
                .FirstOrDefault(a => a.ExamQuestions?.QuestionId == complaint.ExamQuestion.QuestionId);

            return new ComplaintVM
            {
                Id = complaint.Id,
                ExamAttemptId = complaint.ExamAttemptId,
                ExamQuestionId = complaint.ExamQuestionId,
                Comment = complaint.Comment,
                Status = complaint.Status.ToString(),
                Response = complaint.Response,
                CurrentMark = studentAnswer?.StudQMarks ?? 0,
                ExamQuestion = new CreateExamQuestionVM
                {
                    ExamQuestionId = complaint.ExamQuestion.Id,
                    QuestionId = complaint.ExamQuestion.QuestionId,
                    QuestionText = complaint.ExamQuestion.Question?.QuestionText ?? string.Empty,
                    ActualMark = complaint.ExamQuestion.ActualMark,
                },
                ExamAttempt = new StudentExamAttemptVM
                {
                    AttemptId = complaint.ExamAttempt.Id,
                    ExamId = complaint.ExamAttempt.ExamId,
                    StudentId = complaint.ExamAttempt.StudentId,
                    StudResult = complaint.ExamAttempt.StudResult
                }
            };
        }
    }
}