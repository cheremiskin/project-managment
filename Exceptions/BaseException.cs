using System.Text.Json.Serialization;

namespace project_managment.Exceptions
{
    public class BaseException
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("message")] 
        public string Message { get; set; }
    }
}