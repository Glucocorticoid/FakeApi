using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FakeTestApp.Models
{
    public class UserInfoData
    {
        public UserInfoData(string userId, string countSignIn)
        {
            UserId = userId;
            CountSignIn = countSignIn;
        }

        [JsonPropertyName("user_id")]
        public string UserId { get; }

        [JsonPropertyName("count_sign_in")]
        public string CountSignIn { get; }
    }
}
