using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class ExamTopic
    {
        public int Id { get; private set; }

        public int ExamId { get; private set; }

        public int TopicId { get; private set; }

        public Exam Exam { get; private set; } = null!;

        public Topic Topic { get; private set; } = null!;

        protected ExamTopic() { }

        public ExamTopic(int examId, int topicId)
        {
            ExamId = examId;
            TopicId = topicId;
        }
    }
}
