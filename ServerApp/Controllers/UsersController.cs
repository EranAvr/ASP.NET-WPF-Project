using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using ServerAPI.Models;
// third parties:
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json.Nodes;

namespace ServerAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private const string connectionString = "mongodb://localhost:27017";
        private const string dbName = "C_SHARP_FINAL";
        private const string sessionCollName = "sessions";
        private const string usersCollName = "users";
        private const string teachersCollName = "teachers";
        private const string studentsCollName = "students";
        

        public UsersController() {
            var client = new MongoClient(connectionString);
            var db = client.GetDatabase(dbName);
            List<string> dbCollections = db.ListCollectionNames().ToList();
            if (!dbCollections.Contains(sessionCollName))
            {
                db.CreateCollection(sessionCollName);
            }
            if (!dbCollections.Contains(usersCollName))
            {
                db.CreateCollection(usersCollName);
            }

            // For testing purposes-
            CreateTestUsers();
        }

        
        private void CreateTestUsers()
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<MyUser>(usersCollName);
            // check for teacher-
            MyUser doc = collection.Find(user => user.Type == ConstantsRepository.userTypeTeacher).FirstOrDefault();
            if (doc == null) {
                collection.InsertOne(new MyUser(email: "t@mail.com", password: "t", name: "Tim Timmi", type: ConstantsRepository.userTypeTeacher));
            }
            // check for student-
            doc = collection.Find(user => user.Type == ConstantsRepository.userTypeStudent).FirstOrDefault();
            if (doc == null)
            {
                collection.InsertOne(new MyUser(email: "s@mail.com", password: "s", name: "Sam Sammi", type: ConstantsRepository.userTypeStudent));
            }
        }





        /// <summary>
        /// CRUD Operations
        /// </summary>
        /// 
        // GET: UsersController
        [HttpGet]
        [Route("All")]
        public List<MyUser> GetAllUsers()
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<MyUser>(usersCollName);

            List<MyUser> docs = collection.Find(_ => true).ToList();
            return docs;
        }

        // POST: UsersController
        [HttpPost]
        [Route("Add")]
        public void AddUser([FromBody] MyUser user)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<MyUser>(usersCollName);

            var filter = Builders<MyUser>.Filter.Eq("id", user.uId);
            MyUser? search = collection.Find(filter).FirstOrDefault();
            if (search == null)
                collection.InsertOne(user);
        }

        // PUT: UsersController
        [HttpPut]
        [Route("Update/{uid}")]
        public void UpdateUser(string uid, [FromBody] JsonDocument doc)
        {
            JsonElement root = doc.RootElement;

            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<MyUser>(usersCollName);

            var filter = Builders<MyUser>.Filter.Eq("uId", uid);
            var update = Builders<MyUser>.Update.Set("updated", DateTime.UtcNow);
            if (root.TryGetProperty("Email", out JsonElement outE))
            {
                update = update.Set("Email", outE.GetString());
            }
            if (root.TryGetProperty("Password", out JsonElement outP))
            {
                update = update.Set("Password", outP.GetString());
            }
            if (root.TryGetProperty("Name", out JsonElement outN))
            {
                update = update.Set("Name", outN.GetString());
            }
            if (root.TryGetProperty("ExamIDs", out JsonElement outXL))
            {
                for (int i = 0; i < outXL.GetArrayLength(); ++i)
                {
                    collection.UpdateOne(filter, Builders<MyUser>.Update.Push(user => user.ExamIDs, outXL[i].ToString()));
                }
            }

            collection.UpdateOne(filter, update);
        }

        // DELETE: UsersController
        [HttpDelete]
        [Route("Remove/{id}")]
        public void DeleteT(string id)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<MyUser>(usersCollName);

            var filter = Builders<MyUser>.Filter.Eq("uId", id);
            collection.FindOneAndDelete(filter);
        }



        /// <summary>
        /// Login/Logout Operations
        /// </summary>

        [HttpGet]
        [Route("Sessions/Get/{id}")]
        public SessionObj GetSession(string id)
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<SessionObj>(sessionCollName);

            SessionObj session = collection.Find(doc => doc.SessionId == id).FirstOrDefault();
            return session;
        }
        
        [HttpPost]  
        [Route("Sessions/Login")]
        public AuthResponse Login([FromBody] SessionObj session)
        {
            // Validating users existence:
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            // Search for existing user-
            var usersCollection = database.GetCollection<MyUser>(usersCollName);
            var uFilters = Builders<MyUser>.Filter.And(
                Builders<MyUser>.Filter.Eq("email", session.SessionEmail), 
                Builders<MyUser>.Filter.Eq("password", session.SessionPassword));
            MyUser? searchUser = usersCollection.Find(uFilters).FirstOrDefault();
            // Search for existing session-
            var sessionsCollection = database.GetCollection<SessionObj>(sessionCollName);
            var sFilters = Builders<SessionObj>.Filter.And(
                Builders<SessionObj>.Filter.Eq("email", session.SessionEmail),
                Builders<SessionObj>.Filter.Eq("password", session.SessionPassword));
            SessionObj? searchSess = sessionsCollection.Find(sFilters).FirstOrDefault();

            // Return Authentication-Response according to search result:
            if (searchSess != null)
            {
                this.HttpContext.Response.StatusCode = 202; // set status code to 'Accepted' code.
                return new() { isAuth=true, sessionId=session.SessionId, userId=searchUser.uId, userName=searchUser.Name, userEmail=searchUser.Email, userType=searchUser.Type, message= "User already logged in."};
                //return new AuthResponse(true, session.SessionId, searchUser.uId, searchUser.Email, searchUser.Type, message: "User already logged in.");

            }
            if (searchUser != null)
            {
                sessionsCollection.InsertOne(session);  // insert new session to 'sessions' collection.
                this.HttpContext.Response.StatusCode = 201; // set status code to 'Created' code.
                return new() { isAuth = true, sessionId = session.SessionId, userId = searchUser.uId, userName = searchUser.Name, userEmail = searchUser.Email, userType = searchUser.Type, message = "Login is successful." };
                //return new AuthResponse(true, session.SessionId, searchUser.uId, searchUser.Email, searchUser.Type, message: "Login is successful.");
            }
            else
            {
                this.HttpContext.Response.StatusCode = 401; // set status code to 'Unauthorized' code.
                return new() { isAuth=false, message="User not found!" };
            }
        }

        [HttpDelete]
        [Route("Sessions/Logout/{sid}")]
        public string Logout(string sid)
        {
            /*
             * TODO: make func return Json of deleted session
             */
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(dbName);
            var collection = database.GetCollection<SessionObj>(sessionCollName);

            var filter = Builders<SessionObj>.Filter.Eq("sessionId", sid);
            //var projection = Builders<BsonDocument>.Projection.Exclude("_id");
            SessionObj deletedDoc = collection.FindOneAndDelete(filter);
            if (deletedDoc == null)
            {
                // set status code to 'NotFound' code.
                return NotFound().ToJson();
            }
            else {
                // set status code to 'Ok' code.
                return Ok().ToJson();
            }
        }
    }
}
