using System.Text.Json;
using System.Text.Json.Serialization;

namespace ClientGUI.Models
{
    public class AuthResponse
    {
        [JsonPropertyName("isAuth")]
        public bool isAuth { get; set; }
        [JsonPropertyName("sessionId")]
        public string sessionId { get; set; }
        [JsonPropertyName("userId")]
        public string userId { get; set; }
        [JsonPropertyName("userName")]
        public string userName { get; set; }
        [JsonPropertyName("userEmail")]
        public string userEmail { get; set; }
        [JsonPropertyName("userType")]
        public string userType { get; set; }
        [JsonPropertyName("message")]
        public string message { get; set; }

        public AuthResponse(bool isAuth, string sessionId, string userId, string userName, string userEmail, string userType, string message = "")
        {
            this.isAuth = isAuth;
            if(isAuth)
            {
                this.sessionId = sessionId;
                this.userId = userId;
                this.userName = userName;
                this.userEmail = userEmail;
                this.userType = userType;
            }
            else
            {
                this.message = message;
            }
        }
        public override string ToString() {
            return JsonSerializer.Serialize(this);
        }
    }
}
