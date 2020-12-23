using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace pm.Models.UpdateModels
{
    public class UserUpdate
    {
        [JsonPropertyName("info")]
        [StringLength(512)]
        public string Info { get; set; }
        
        [StringLength(128)]
        // [RegularExpression("^[A-Z][a-zA-Z]{3,}(?: [A-Z][a-zA-Z]*){0,2}$")]
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }

        public DateTime? BirthDate { get; set; }

        public User ToUser(long id)
        {
            return new User
            {
                FullName = this.FullName,
                Info = this.Info,
                BirthDate = this.BirthDate,
                Id = id
            };
        }
    }
}