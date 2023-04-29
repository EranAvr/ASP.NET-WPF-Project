using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace ClientGUI.Models
{
    public record ExamBaseInfo
    {
        // JSON props:

        [JsonPropertyName("examName")]
        public string Name { get; set; }
        [JsonPropertyName("examAuthor")]
        public string Author { get; set; }
        [JsonPropertyName("examAuthorID")]
        public string AuthorID { get; set; }
        [JsonPropertyName("examDate")]
        public DateTime Date { get; set; }
        [JsonPropertyName("examStart")]
        public string TimeStart { get; set; }
        [JsonPropertyName("examDuration")]
        public int TimeDuration { get; set; }
        [JsonPropertyName("examRandom")]
        public bool Randomize { get; set; }

        public ExamBaseInfo() {}
        public void Clear()
        {
            Name = string.Empty;
            Author = string.Empty;
            Date = new DateTime();
            TimeStart = "00:00";
            TimeDuration = 0;
            Randomize = false;
        }
        // We overrided ToString() method to fit it to our use-
        public override string ToString()
        {
            return $"Exam Name: {Name}\n" +
                $"Exam Author: {Author}\n" +
                $"Date: {Date.ToString("dddd, dd MMMM yyyy")}\n" +
                $"Start Time: {TimeStart}\n" +
                $"Duration: {TimeDuration}";
        }
    }
    public class Exam
    {
        [JsonPropertyName("examId")]
        public string examId { get; set; }
        [JsonPropertyName("info")]
        public ExamBaseInfo Info { get; set; }
        [JsonPropertyName("questions")]
        public List<Question> Questions { get; set; }

        // methods:
        public Exam(ExamBaseInfo info)
        {
            examId = Guid.NewGuid().ToString();
            Info = info;
            Questions = new List<Question>();
        }

        // Info CRUD operations:
        public void UpdateInfo(ExamBaseInfo info)
        {
            Info.Name = info.Name;
            Info.Author = info.Author;
            Info.Date = info.Date;
            Info.TimeStart = info.TimeStart;
            Info.TimeDuration = info.TimeDuration;
            Info.Randomize = info.Randomize;
        }

        // Questions CRUD operations:
        public void AddQuestion(Question q)
        {
            Questions.Add(q);
        }
        public List<Question> GetAllQuestions()
        {
            return Questions;
        }
        public Question GetQuestionByID(string q_id)
        {
            int index = Questions.FindIndex(q => q.QID == q_id);
            if (index >= -1)
            {
                return Questions[index];
            }
            return null;
        }
        public void UpdateQuestion(Question question)
        {
            /*
             * use FindIndex method or LINQ query
             */
            int index = Questions.FindIndex(q => q.QID == question.QID);
            if (index >= -1)
            {
                Questions[index].QText = question.QText;
                Questions[index].CorrectAnswer = question.CorrectAnswer;
                Questions[index].Answers = question.Answers;
                Questions[index].Randomize = question.Randomize;
            }
        }
        public void DeleteQuestion(string q_id)
        {
            int index = Questions.FindIndex(q => q.QID == q_id);
            if (index >= -1)
            {
                Questions.RemoveAt(index);
            }
        }

        public override string ToString()
        {
            return $"{Info.Name}, {Info.Date.ToString("dd/MM/yyyy")}";
        }
    }
    /*
    public record StudentInfo
    {
        public string ID { get; set; }
        public string FullName { get; set; }
    }
    */
    public class StudentExam
    {
        // Properties:
        [JsonPropertyName("studentID")]
        public string StudentID { get; set; }
        [JsonPropertyName("examID")]
        public string ExamID { get; set; }
        [JsonPropertyName("bInfo")]
        public ExamBaseInfo BInfo { get; set; }
        [JsonPropertyName("answers")]
        public List<Question> StudentAnswers { get; set; }
        //public StudentInfo StudentInfo { get; }

        // Constructor:
        public StudentExam()
        {
        }
        public StudentExam(string StudentID, Exam baseExam) 
        {
            // References to student and original exam-
            this.StudentID = StudentID;
            ExamID = Guid.NewGuid().ToString();
            // save info and questions from baseExam,
            // and initialize other properties.
            BInfo = baseExam.Info;
            // set student answers form-
            StudentAnswers = new List<Question>();
            foreach (Question q in baseExam.GetAllQuestions()) {
                StudentAnswers.Add(q);
                StudentAnswers.Last().CorrectAnswer = null;
            }
        }
        

        // Methods:
        public void SetCorrectAnswer(string q_id, string answer)
        {
            StudentAnswers.Find(q => q.QID == q_id).CorrectAnswer = answer;
        }
        public void DeleteAnswer(string q_id)
        {
            StudentAnswers.Find(q => q.QID == q_id).CorrectAnswer = null;
        }
        public List<Question> GetAllQuestions()
        {
            return StudentAnswers;
        }
        public Question GetQuestionByID(string q_id)
        {
            int index = StudentAnswers.FindIndex(q => q.QID == q_id);
            if (index >= -1)
            {
                return StudentAnswers[index];
            }
            return null;
        }

        public override string ToString()
        {
            return $"Exam Name: {BInfo.Name}\n" +
                $"Date: {BInfo.Date}\n" +
                $"Start Time: {BInfo.TimeStart}\n" +
                $"Duration: {BInfo.TimeDuration}";
        }
    }
    
}
