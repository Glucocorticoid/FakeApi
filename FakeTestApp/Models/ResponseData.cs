using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace FakeTestApp.Models
{
    public class ResponseData
    {
        public ResponseData(string query, int percent, UserInfoData result = null)
        {
            Query = query;
            Percent = percent;
            Result = result;
        }

        [JsonPropertyName("query")]
        public string Query { get; }

        [JsonPropertyName("percent")]
        public int Percent { get; }

        [JsonPropertyName("result")]
        public UserInfoData Result { get; }
    }
}
