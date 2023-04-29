using System.Text.Json.Serialization;
using System.Text.Json;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerAPI.Models
{
    public class MyUser
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        [BsonElement("uId")]
        [JsonPropertyName("uId")]
        public string uId { get; set; }
        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [BsonElement("name")]
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [BsonElement("type")]
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [BsonElement("examIDs")]
        [JsonPropertyName("examIDs")]
        public List<string> ExamIDs { get; set; }
        [BsonElement("updated")]
        public DateTime updated { get; set; }

        public MyUser(string email, string password, string name, string type)
        {
            uId = Guid.NewGuid().ToString();
            Email = email;
            Password = password;
            Name = name;
            Type = type;
        }

        public override string ToString()
        {
            return uId +" "+ Email + " " + Password + " " + Name;
        }
    }
}
