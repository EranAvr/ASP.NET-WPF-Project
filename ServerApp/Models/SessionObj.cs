using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerAPI.Models
{
    public class SessionObj
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        [BsonElement("sessionId")]
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }
        [BsonElement("email")]
        [JsonPropertyName("email")]
        public string SessionEmail { get; set; }
        [BsonElement("password")]
        [JsonPropertyName("password")]
        public string SessionPassword { get; set; }

        public SessionObj() {}
        public SessionObj(string email, string pass)
        {
            SessionId = Guid.NewGuid().ToString();
            SessionEmail = email;
            SessionPassword = pass;
        }
    }
}
