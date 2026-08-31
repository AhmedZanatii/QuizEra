using QuizEra.BLL.ModelVM.Feedback;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Repositories.Abstraction;
using QuizEra.DAL.Entities;

namespace QuizEra.BLL.Services.Implementation
{
    public class FeedbackService : IFeedbackService
    {
        private IGenericRepository<Feedback> FeedbackRepo;
        private IGenericRepository<Student> StudentRepo;

        public FeedbackService(IGenericRepository<Feedback> feedbackRepo, IGenericRepository<Student> studentRepo)
        {
            FeedbackRepo = feedbackRepo;
            StudentRepo = studentRepo;
        }

        public async Task<IEnumerable<FeedbackVM>> GetByStudentIdAsync(string id)
        {
            // Get the Student from the database
            var student = await StudentRepo.GetBy(
                filter: s => s.AppUserId == id,
                noTrack: true
            );

            // Retrieve all feedbacks for the given StudentID
            var feedbacks = (await FeedbackRepo.Get(a => a.StudentID == student.Id)).Where(a => !a.IsDeleted).ToList();

            // Check if any feedbacks were found
            if(feedbacks == null || !feedbacks.Any())
            {
                throw new Exception($"No Feedback found for StudentID {id}");
            }

            // Map the retrieved feedbacks to FeedbackVM
            return feedbacks.Select(MapToVM);
        }
        public async Task<IEnumerable<FeedbackVM>> GetByCourseIdAsync(int id)
        {
            // Retrieve all feedbacks for the given CourseID
            var feedbacks = (await FeedbackRepo.Get(a => a.CourseID == id)).Where(a => !a.IsDeleted).ToList();

            // Check if any feedbacks were found
            if(feedbacks == null || !feedbacks.Any())
            {
                throw new Exception($"No Feedback found for CourseID {id}");
            }

            // Map the retrieved feedbacks to FeedbackVM
            return feedbacks.Select(MapToVM);
        }

        public async Task<FeedbackVM> GetByIdAsync(int id)
        {
            // Retrieve feedback for the given Id
            var feedback = (await FeedbackRepo.Get(a => a.Id == id)).FirstOrDefault(a => !a.IsDeleted);

            // Check if feedback were found
            if(feedback == null)
            {
                throw new Exception($"No Feedback found for ID {id}");
            }

            // Map the retrieved feedback to FeedbackVM
            return MapToVM(feedback);
        }

        public async Task AddAsync(FeedbackVM feedback, string creatorUser)
        {
            // Validate the input
            if(feedback == null || feedback.Rate <= 0) 
            {
                throw new ArgumentException("Invalid feedback provided for addition.");
            }

            // Get the Student from the database
            var student = await StudentRepo.GetBy(
                filter: s => s.AppUserId == feedback.StudentID,
                noTrack: true
            );

            // Create a new Feedback and add it to the repository
            await FeedbackRepo.Create(new Feedback(student.Id, feedback.CourseID, 
                                feedback.Comment, feedback.Rate, creatorUser));
            await FeedbackRepo.SaveAsync();
        }
        public async Task UpdateAsync(FeedbackVM feedback, string modifierUser)
        {
            // Validate the input
            if(feedback == null || feedback.Rate < 0) 
            {
                throw new ArgumentException("Invalid feedback provided for addition.");
            }

            // Retrieve the existing Feedback from the repository
            var existingFeedback = (await FeedbackRepo.Get(a => a.Id == feedback.Id)).FirstOrDefault();
            if (existingFeedback == null)
            {
                throw new Exception($"Feedback not found for ID {feedback.Id}");
            }

            // Update the existing feedback with the new values
            existingFeedback.Update(feedback.Comment, feedback.Rate, modifierUser);
            FeedbackRepo.Update(existingFeedback);
            await FeedbackRepo.SaveAsync();
        }

        public async Task DeleteAsync(int id, string deleterUser) 
        {
            // Retrieve the existing Feedback from the repository
            var existingFeedback = (await FeedbackRepo.Get(a => a.Id == id)).FirstOrDefault();
            if (existingFeedback == null)
            {
                throw new Exception($"Feedback not found for ID {id}");
            }

            // Mark the feedback as deleted
            existingFeedback.Delete(deleterUser, DateTime.Now);
            FeedbackRepo.Delete(existingFeedback);
            await FeedbackRepo.SaveAsync();
        }
    
        // Minimal helper to map entity collection to VM collection securely
        private static FeedbackVM MapToVM(Feedback a)
        {
            return new FeedbackVM
            {
                Id = a.Id,
                CourseID = a.CourseID,
                Comment = a.Comment,
                Rate = a.Rate
            };
        }
    }
}