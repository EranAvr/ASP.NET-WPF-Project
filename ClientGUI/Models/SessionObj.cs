using System;
using System.Text.Json.Serialization;

namespace ClientGUI.Models
{
    public class SessionObj
    {
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }
        [JsonPropertyName("email")]
        public string SessionEmail { get; set; }
        [JsonPropertyName("password")]
        public string SessionPassword { get; set; }

        public SessionObj() { }
        public SessionObj(string email, string pass)
        {
            SessionId = Guid.NewGuid().ToString();
            SessionEmail = email;
            SessionPassword = pass;
        }
    }
}
