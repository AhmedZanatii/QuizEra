using QuizEra.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Question
    {
        public int QuestionID { get; private set; }
        public int TopicID { get; private set; } 
        public string QuestionText { get; private set; }
        public string QuestionType { get; private set; }
        public string QuestionAnswer { get; private set; }
        public DifficultyLevel DifficultyLevel { get; private set; }
        public string Photo { get; private set; }

        // Navigation Properties
        public Topic Topic { get; private set; } 
        public ICollection<ExamQuestions> ExamQuestions { get; private set; } = new List<ExamQuestions>();

        protected Question() { }

        public Question(int topicID, string questionText, string questionType, string questionAnswer, DifficultyLevel difficultyLevel, string photo)
        {
            TopicID = topicID;
            QuestionText = questionText;
            QuestionType = questionType;
            QuestionAnswer = questionAnswer;
            DifficultyLevel = difficultyLevel;
            Photo = photo;
        }

        public void Update(int topicID, string questionText, string questionType, string questionAnswer, DifficultyLevel difficultyLevel, string photo)
        {
            TopicID = topicID;
            QuestionText = questionText;
            QuestionType = questionType;
            QuestionAnswer = questionAnswer;
            DifficultyLevel = difficultyLevel;
            Photo = photo;
        }
    }
}
