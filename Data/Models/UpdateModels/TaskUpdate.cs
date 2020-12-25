using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pm.Models.UpdateModels
{
    public class TaskUpdate
    {
        [JsonPropertyName("title")]
        [StringLength(128, MinimumLength = 1)]
        public string Title { get; set; } 
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("expirationDate")]
        public DateTime? ExpirationDate { get; set; }
        [JsonPropertyName("executionTime")]
        public DateTime? ExecutionTime { get; set; }
        [JsonPropertyName("statusId")]
        public int StatusId { get; set; }

        public Task ToTask(long id)
        {
            return new Task
            {
                Title = this.Title, 
                Content = this.Content,
                ExpirationDate = this.ExpirationDate,
                ExecutionTime = this.ExecutionTime,
                StatusId = this.StatusId,
                Id = id
            };
        }
        
    }
}