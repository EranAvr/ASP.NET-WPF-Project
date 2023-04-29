using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using ServerAPI.Models;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExamsController : ControllerBase
    {
        private readonly string connString = ConstantsRepository.serverConnectionString;
        private readonly string dbName = ConstantsRepository.serverDbName;
        private const string eCollName = "exams";
        private const string solvedCollName = "solved_exams";
        private const string qCollName = "questions";

        public ExamsController()
        {
            var client = new MongoClient(connString);
            var db = client.GetDatabase(dbName);
            List<string> dbCollections = db.ListCollectionNames().ToList();

            if (!dbCollections.Contains(ConstantsRepository.serverExamsCollName))
                db.CreateCollection(ConstantsRepository.serverExamsCollName);
            if (!dbCollections.Contains(ConstantsRepository.serverSolvedCollName))
                db.CreateCollection(ConstantsRepository.serverSolvedCollName);
            if (!dbCollections.Contains(ConstantsRepository.serverStudentsGrades))
                db.CreateCollection(ConstantsRepository.serverStudentsGrades);
        }
        /// <summary>
        /// Questions Collection API:
        /// /// </summary>
        /*
        // GET
        [HttpGet]
        [Route("Questions")]
        public List<Question> GetQuestions()
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Question>(qCollName);

            List<Question> docs = collection.Find(_ => true).ToList();
            return docs;
        }
        // POST
        [HttpPost]
        [Route("AddQuestion")]
        public void AddQuestion([FromBody] Question q)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Question>(qCollName);

            var filter = Builders<Question>.Filter.Eq("id", q.QID);
            Question? search = collection.Find(filter).FirstOrDefault();
            if (search == null)
                collection.InsertOne(q);
        }
        // PUT
        [HttpPut]
        [Route("EditQuestion/{id}")]
        public void EditQuestion(string id, [FromBody] Question q)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Question>(qCollName);

            var filter = Builders<Question>.Filter.Eq("id", q.QID);
            collection.ReplaceOne(filter, q);
        }
        // DELETE
        [HttpDelete]
        [Route("RemoveQuestion/{id}")]
        public void DeleteQuestion(string id)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Question>(qCollName);

            var filter = Builders<Question>.Filter.Eq("Id", id);
            collection.FindOneAndDelete(filter);
        }
        */



        /// <summary>
        /// Teachers Exams API:
        /// /// </summary>
        // GET
        [HttpGet]
        [Route("Templates/Info/{name}")]
        public ExamBaseInfo GetInfo(string name)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Exam>(eCollName);

            var filters = Builders<Exam>.Filter.Eq("Info.Name", name)
                | Builders<Exam>.Filter.StringIn("Info.Name", name);
            Exam ex = collection.Find(filters).FirstOrDefault();
            if (ex != null)
                return ex.Info;

            return null;
        }
        [HttpGet]
        [Route("Templates/All")]
        public List<Exam> GetExams()
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Exam>(eCollName);

            List<Exam> docs = collection.Find(_ => true).ToList();
            return docs;
        }
        [HttpGet]
        [Route("Templates/Of/{uName}")]
        public List<Exam> GetOfTeacher(string uName)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var uColl = database.GetCollection<MyUser>(ConstantsRepository.usersCollName);
            //check if user exists-
            var filter = Builders<MyUser>.Filter.Eq("name", uName);
            MyUser doc = uColl.Find(filter).FirstOrDefault();
            if (doc != null && doc.Type == ConstantsRepository.userTypeTeacher) {
                var eColl = database.GetCollection<Exam>(ConstantsRepository.serverExamsCollName);
                var filter2 = Builders<Exam>.Filter.Eq("Info.Author", uName);
                List<Exam> docs = eColl.Find(filter2).ToList();
                return docs;
            }
            return null;
        }
        [HttpGet]
        [Route("Templates/ById/{id}")]
        public Exam GetById(string id)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Exam>(eCollName);

            var filter = Builders<Exam>.Filter.Eq("examId", id);
            Exam doc = collection.Find(filter).SingleOrDefault();
            return doc;
        }
        [HttpGet]
        [Route("Templates/ByName/{name}")]
        public Exam GetByName(string name)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Exam>(eCollName);

            var filters = Builders<Exam>.Filter.Eq("Info.Name", name) 
                | Builders<Exam>.Filter.StringIn("Info.Name", name);
            Exam doc = collection.Find(filters).FirstOrDefault();
            return doc;
        }
        // POST
        [HttpPost]
        [Route("Templates/Add")]
        public ActionResult AddExam([FromBody] Exam exam)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Exam>(eCollName);

            var filter = Builders<Exam>.Filter.Eq("examId", exam.examId);
            Exam? search = collection.Find(filter).FirstOrDefault();
            if (search == null)
            {
                collection.InsertOne(exam);  // insert new session to 'sessions' collection.
                return Created("", "Exam saved in server"); // set status code to 'Created' code.
            }
            else
            {
                return BadRequest($"Error! Exam already exists in server\nID: {exam.examId}"); // set status code to 'Unauthorized' code.
            }
        }
        // PUT
        [HttpPut]
        [Route("Templates/Update/{id}")]
        public void PutExam(string id, [FromBody] Exam exam)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Exam>(eCollName);

            var filter = Builders<Exam>.Filter.Eq("examId", exam.examId);
            collection.ReplaceOne(filter, exam);
        }
        // DELETE
        [HttpDelete]
        [Route("Templates/Remove/{id}")]
        public void DeleteExam(string id)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<Exam>(eCollName);

            var filter = Builders<Exam>.Filter.Eq("examId", id);
            collection.FindOneAndDelete(filter);
        }




        /// <summary>
        /// Students Exams API:
        /// </summary>
        // GET
        [HttpGet]
        [Route("Solved/ById/{id}")]
        public StudentExam StudentExamById(string id)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<StudentExam>(solvedCollName);

            var filter = Builders<StudentExam>.Filter.Eq("ExamID", id);
            StudentExam doc = collection.Find(filter).SingleOrDefault();
            return doc;
        }
        [HttpGet]
        [Route("Solved/GetNew/{uid}/{name}")]
        public StudentExam CreateStudentExam(string uid, string name)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);

            var eColl = database.GetCollection<Exam>(eCollName);
            var eFilters = Builders<Exam>.Filter.Eq("Info.Name", name)
                | Builders<Exam>.Filter.StringIn("Info.Name", name);
            Exam ex = eColl.Find(eFilters).FirstOrDefault();

            var uColl = database.GetCollection<MyUser>(ConstantsRepository.usersCollName);
            var filters = Builders<MyUser>.Filter.Eq("uId", uid);
            MyUser us = uColl.Find(filters).FirstOrDefault();

            return new StudentExam(us.uId, ex);
        }

        // POST
        [HttpPost]
        [Route("Solved/Submit")]
        public void SubmitExam([FromBody] StudentExam exam)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<StudentExam>(ConstantsRepository.serverSolvedCollName);

            var filters = Builders<StudentExam>.Filter.Eq("studentID", exam.StudentID)
                & Builders<StudentExam>.Filter.Eq("bInfo.Name", exam.BInfo.Name);
            StudentExam? search = collection.Find(filters).FirstOrDefault();
            if (search == null)
            {
                collection.InsertOne(exam);
                SubmitExamForGrade(exam);
                this.HttpContext.Response.StatusCode = 201;
            }
            else
            {
                this.HttpContext.Response.StatusCode = 401;
            }
        }
        [HttpPost]
        [Route("Solved/SubmitForGrade")]
        public ExamGrade SubmitExamForGrade([FromBody] StudentExam studentE)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var eColl = database.GetCollection<Exam>(ConstantsRepository.serverExamsCollName);

            var filter = Builders<Exam>.Filter.Eq("Info.Name", studentE.BInfo.Name);
            Exam? originalE = eColl.Find(filter).SingleOrDefault();
            if (originalE != null)
            {
                var gColl = database.GetCollection<ExamGrade>(ConstantsRepository.serverStudentsGrades);
                ExamGrade grade = new ExamGrade(originalE, studentE);
                gColl.InsertOne(grade);
                return grade;
            }
            else
            {
                return null;
            }
        }
        // DELETE
        [HttpDelete]
        [Route("Solved/Remove/{uid}/{eid}")]
        public StudentExam RemoveStudentExam(string uid, string eid)
        {
            var client = new MongoClient(connString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<StudentExam>(solvedCollName);

            var filters = Builders<StudentExam>.Filter.Eq("studentID", uid)
                & Builders<StudentExam>.Filter.Eq("examID", eid);
            return collection.FindOneAndDelete(filters);
        }
    }
}
