using QuizEra.BLL.ModelVM.Questions;
using QuizEra.BLL.ModelVM.Topic;
using QuizEra.BLL.Services.Abstraction;
using QuizEra.DAL.Entities;
using QuizEra.DAL.Repositories.Abstraction;

namespace QuizEra.BLL.Services
{
    public class TopicService : ITopicService
    {
        private readonly IGenericRepository<Topic> _topicRepository;
        private readonly IGenericRepository<Question> _questionRepository;

        public TopicService(
            IGenericRepository<Topic> topicRepository,
            IGenericRepository<Question> questionRepository)
        {
            _topicRepository = topicRepository;
            _questionRepository = questionRepository;
        }

        // =====================================================
        // Get All Active Topics
        // =====================================================

        public async Task<IEnumerable<TopicVM>> GetAllTopicsAsync()
        {
            var topics = await _topicRepository.Get(
                filter: t => !t.IsDeleted,
                noTrack: true
            );

            return topics.Select(t => new TopicVM
            {
                Id = t.Id,
                CourseId = t.CourseID,
                Name = t.Name,
                IsDeleted = t.IsDeleted
            });
        }

        // =====================================================
        // Get All Topics Including Deleted
        // =====================================================

        public async Task<IEnumerable<TopicVM>> GetAllTopicsIncludingDeletedAsync()
        {
            var topics = await _topicRepository.Get(
                filter: null,
                noTrack: true
            );

            return topics.Select(t => new TopicVM
            {
                Id = t.Id,
                CourseId = t.CourseID,
                Name = t.Name,
                IsDeleted = t.IsDeleted

            });
        }

        // =====================================================
        // Get Topic By ID
        // =====================================================

        public async Task<TopicVM?> GetTopicByIdAsync(int id)
        {
            var topic = await _topicRepository.GetBy(
                filter: t => t.Id == id && !t.IsDeleted,
                noTrack: true
            );

            if (topic == null)
                return null;

            return new TopicVM
            {
                Id = topic.Id,
                CourseId = topic.CourseID,
                Name = topic.Name,
                IsDeleted = topic.IsDeleted

            };
        }

        // =====================================================
        // Get Topics By Course
        // =====================================================

        public async Task<IEnumerable<TopicVM>> GetTopicsByCourseAsync(int courseId)
        {
            var topics = await _topicRepository.Get(
                filter: t => t.CourseID == courseId && !t.IsDeleted,
                noTrack: true
            );

            return topics.Select(t => new TopicVM
            {
                Id = t.Id,
                CourseId = t.CourseID,
                Name = t.Name
            });
        }

        // =====================================================
        // Create Topic
        // =====================================================

        public async Task<bool> CreateTopicAsync(CreateTopicVM createVM)
        {
            var topic = new Topic(
                courseID: createVM.CourseId,
                name: createVM.Name,
                creatorUser: createVM.CreatorUser
            );

            await _topicRepository.Create(topic);
            await _topicRepository.SaveAsync();

            return true;
        }

        // =====================================================
        // Update Topic
        // =====================================================

        public async Task<bool> UpdateTopicAsync(UpdateTopicVM updateVM)
        {
            var topic = await _topicRepository.GetBy(
                filter: t => t.Id == updateVM.Id && !t.IsDeleted,
                noTrack: false
            );

            if (topic == null)
                return false;

            topic.Update(
                courseID: updateVM.CourseId,
                name: updateVM.Name,
                modifierUser: updateVM.ModifierUser
            );

            _topicRepository.Update(topic);

            await _topicRepository.SaveAsync();

            return true;
        }

        // =====================================================
        // Delete Topic
        // =====================================================

        public async Task<bool> DeleteTopicAsync(
            int id,
            string deleterUser)
        {
            var topic = await _topicRepository.GetBy(
                filter: t => t.Id == id && !t.IsDeleted,
                noTrack: false
            );

            if (topic == null)
                return false;

            bool isDeleted =
                topic.Delete(deleterUser, DateTime.UtcNow);

            if (!isDeleted)
                return false;

            _topicRepository.Update(topic);

            await _topicRepository.SaveAsync();

            return true;
        }

        // =====================================================
        // Restore Topic
        // =====================================================

        public async Task<bool> RestoreTopicAsync(
            int id,
            string modifierUser)
        {
            var topic = await _topicRepository.GetBy(
                filter: t => t.Id == id && t.IsDeleted,
                noTrack: false
            );

            if (topic == null)
                return false;

            topic.Restore(modifierUser);

            _topicRepository.Update(topic);

            await _topicRepository.SaveAsync();

            return true;
        }

        // =====================================================
        // Topic Details
        // =====================================================

        public async Task<TopicDetailsVM?> GetTopicDetailsAsync(
            int topicId)
        {
            var topic = await _topicRepository.GetBy(
                filter: t => t.Id == topicId && !t.IsDeleted,
                noTrack: true
            );

            if (topic == null)
                return null;

            var questions = await _questionRepository.Get(
                filter: q => q.TopicID == topicId,
                noTrack: true
            );

            var questionVMs = questions.Select(q => new QuestionVM
            {
                Id = q.Id,
                TopicID = q.TopicID,
                QuestionText = q.QuestionText,
                QuestionFormat = q.QuestionFormat,
                QuestionAnswer = q.QuestionAnswer,
                DifficultyLevel = q.DifficultyLevel,
                Photo = q.Photo,
                IsDeleted = q.IsDeleted
            });

            return new TopicDetailsVM
            {
                Id = topic.Id,
                CourseId = topic.CourseID,
                Name = topic.Name,
                Questions = questionVMs
            };
        }
    }
}