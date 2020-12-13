using pm.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace project_managment.Forms
{
    public class CreateProjectForm
    {
        [Required]
        [JsonPropertyName("name")]
        [StringLength(255, MinimumLength = 1)]
        public string Name { get; set; }

        [JsonPropertyName("description")]
        public string Description { get; set; }
        
        [JsonPropertyName("isPrivate")]
        public bool? IsPrivate { get; set; } = false;
    
        public Project ToProject()
        {
            return  new Project
            {
                Name = Name, Description = Description, CreatedAt = DateTime.Now, IsPrivate = IsPrivate ?? false
            };
        }
    }
}
