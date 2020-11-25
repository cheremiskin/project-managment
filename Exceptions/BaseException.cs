using System;
using System.Net;
using System.Text.Json.Serialization;

namespace project_managment.Exceptions
{
    public class BaseException : Exception
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }
        [JsonPropertyName("message")] 
        public string Message { get; set; }
        [JsonIgnore]
        public HttpStatusCode StatusCode { get; set; }
    }
}