using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ServerAPI.Models
{
    public record Question
    {
        [JsonPropertyName("qId")]
        public string QID { get; set; }
        [JsonPropertyName("qText")]
        public string? QText { get; set; }
        [JsonPropertyName("qPic")]
        public byte[]? QPic { get; set; }
        [JsonPropertyName("qCorrectAns")]
        public string? CorrectAnswer { get; set; }
        [JsonPropertyName("qAnswers")]
        public List<string> Answers { get; set; }
        //private Dictionary<string, string>? Answers { get; set; }
        [JsonPropertyName("qRandom")]
        public bool Randomize { get; set; }

        public Question()
        {
            QID = Guid.NewGuid().ToString();
            Answers = new List<string>();
        }

        public void Clear()
        { 
            QText = string.Empty;
            QPic = null;
            CorrectAnswer = null;
            Answers = new List<string>();
            Randomize = false;
        }
        public void ResetCorrectAnswer()
        {
            CorrectAnswer = null;
        }

        public override string ToString()
        {
            return $"Question: {QText} {CorrectAnswer}";
        }
    }
}
