using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ServerAPI.Models
{
    public class AuthResponse
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        [JsonIgnore]
        public ObjectId Id { get; set; }
        [BsonElement("isAuth")]
        [JsonPropertyName("isAuth")]
        public bool isAuth { get; set; }
        [BsonElement("sessionId")]
        [JsonPropertyName("sessionId")]
        public string sessionId { get; set; }
        [BsonElement("userId")]
        [JsonPropertyName("userId")]
        public string userId { get; set; }
        [BsonElement("userName")]
        [JsonPropertyName("userName")]
        public string userName { get; set; }
        [BsonElement("userEmail")]
        [JsonPropertyName("userEmail")]
        public string userEmail { get; set; }
        [JsonPropertyName("userType")]
        public string userType { get; set; }
        [BsonElement("message")]
        [JsonPropertyName("message")]
        public string message { get; set; }

        public AuthResponse() { }
        public AuthResponse(bool isAuth, string sessionId = "", string userId = "", string userEmail = "", string userType = "", string message = "")
        {
            this.isAuth = isAuth;
            if(isAuth)
            {
                this.sessionId = sessionId;
                this.userId = userId;
                this.userEmail = userEmail;
                this.userType = userType;
                this.message = message;
            }
            else
            {
                this.message = message;
            }
        }
    }
}
