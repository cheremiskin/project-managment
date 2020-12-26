using System;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Text.Json.Serialization;
using pm.Models;

namespace project_managment.Forms
{
    public class CreateTaskForm
    {
        [JsonPropertyName("title")]
        [StringLength(128, MinimumLength = 1)]
        [Required]
        public string Title { get; set; }
        [JsonPropertyName("statusId")]
        public int StatusId { get; set; }
        [Required]
        [JsonPropertyName("content")]
        public string Content { get; set; }
        [JsonPropertyName("expirationDate")]
        public DateTime ExpirationDate { get; set; }
        [JsonPropertyName("executionTime")]
        public DateTime ExecutionTime { get; set; }
        
        [JsonPropertyName("assignedUsers")]
        public long[] AssignedUsers { get; set; }

        public Task ToTask()
        {
            var task = new Task
            {
                Title = this.Title,
                StatusId = this.StatusId == 0 ? 1 : this.StatusId,
                Content = this.Content,
                CreationDate = DateTime.Now,
                ExpirationDate = this.ExpirationDate,
                ExecutionTime = this.ExecutionTime,
            };

            return task;
        }
    }
}