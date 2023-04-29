using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ServerAPI.Models
{
    public record MistakeDetails {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        [JsonPropertyName("q")]
        public string question { get; set; }
        [JsonPropertyName("rAns")]
        public string rightAns { get; set; }
        [JsonPropertyName("wAns")]
        public string wrongAns { get; set; }
    }
    public class ExamGrade
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        [JsonPropertyName("ID")]
        public string ID { get; set; }
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
        [JsonPropertyName("examID")]
        public string ExamID { get; set; }
        [JsonPropertyName("grade")]
        public float Grade { get; set; }
        [JsonPropertyName("mistakes")]
        public List<MistakeDetails> Mistakes { get; set; }

        //public ExamGrade() { }
        public ExamGrade(Exam original, StudentExam checkExam)
        {
            ID = checkExam.StudentID;
            ExamID = original.examId;
            Mistakes = new List<MistakeDetails>();
            float baseScore = 100 / original.Questions.Count();
            float scoreSum = 0;
            foreach (Question q in original.GetAllQuestions()) {
                string studentAns = checkExam.GetQuestionByID(q.QID).CorrectAnswer;
                if (q.CorrectAnswer == studentAns) {
                    // good answer
                    scoreSum += baseScore;
                }
                else {
                    // wrong answer
                    Mistakes.Add(new() { 
                        question = q.QText,
                        rightAns = q.CorrectAnswer,
                        wrongAns = studentAns
                    });;
                }
            }
            Grade = scoreSum;
        }
    }
}
