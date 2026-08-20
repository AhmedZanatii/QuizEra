using QuizEra.BLL.ModelVM.Topic;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.BLL.Services.Abstraction
{
    public interface ITopicService
    {
        Task<IEnumerable<TopicVM>> GetAllTopicsAsync();
        Task<TopicVM?> GetTopicByIdAsync(int id);
        Task<TopicDetailsVM?> GetTopicDetailsAsync(int topicId);
        Task<IEnumerable<TopicVM>> GetTopicsByCourseAsync(int courseId);
        Task<bool> CreateTopicAsync(CreateTopicVM createVM);
        Task<bool> UpdateTopicAsync(UpdateTopicVM updateVM);
        Task<bool> DeleteTopicAsync(int id, string deleterUser);
    }
}
