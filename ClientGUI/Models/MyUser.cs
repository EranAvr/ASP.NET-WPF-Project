using System.Text.Json.Serialization;
using System.Text.Json;
using System;
using System.Collections.Generic;

namespace ClientGUI.Models
{
    public class MyUser
    {

        [JsonPropertyName("uId")]
        public string uId { get; set; }
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; }
        [JsonPropertyName("type")]
        public string Type { get; set; }
        [JsonPropertyName("examIDs")]
        public List<string> ExamIDs { get; set; }
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
