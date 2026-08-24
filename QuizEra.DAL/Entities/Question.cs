using QuizEra.DAL.Entities.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace QuizEra.DAL.Entities
{
    public class Question: BaseEntity
    {
        public int Id { get; private set; }
        public int TopicID { get; private set; } 
        public string QuestionText { get; private set; }
        
        public QuestionFormat QuestionFormat { get; private set; }  //MSQ or T/F or essay
        public string QuestionAnswer { get; private set; }
        public DifficultyLevel DifficultyLevel { get; private set; }
        public string Photo { get; private set; }

        // Navigation Properties
        public Topic Topic { get; private set; } 
        public ICollection<ExamQuestions> ExamQuestions { get; private set; } = new List<ExamQuestions>();

        //to have many options
        public ICollection<QuestionOption> Options { get; private set; } = new List<QuestionOption>();

        protected Question() { }

        public Question(
    int topicID,
    string questionText,
    QuestionFormat questionFormat,
    string questionAnswer,
    DifficultyLevel difficultyLevel,
    string photo,
    string creatorUser)
    : base(creatorUser)
        {
            TopicID = topicID;
            QuestionText = questionText;
            QuestionFormat = questionFormat;
            QuestionAnswer = questionAnswer;
            DifficultyLevel = difficultyLevel;
            Photo = photo;
        }

        public void Update(
     int topicID,
     string questionText,
     QuestionFormat questionFormat,
     string questionAnswer,
     DifficultyLevel difficultyLevel,
     string photo,
     string modifierUser)
        {
            TopicID = topicID;
            QuestionText = questionText;
            QuestionFormat = questionFormat;
            QuestionAnswer = questionAnswer;
            DifficultyLevel = difficultyLevel;
            Photo = photo;

            base.Update(modifierUser);
        }
    }
}
 