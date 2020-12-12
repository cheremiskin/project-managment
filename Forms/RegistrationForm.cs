using DevOne.Security.Cryptography.BCrypt;
using pm.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace project_managment.Forms
{
    public class RegistrationForm
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }
        [Required]
        [JsonPropertyName("passwordConfirm")] 
        public string PasswordConfirm { get; set; }
        [JsonPropertyName("info")]
        public string Info { get; set; }
        
        [Required]
        [StringLength(128)]
        [RegularExpression("^[A-Z][a-zA-Z]{3,}(?: [A-Z][a-zA-Z]*){0,2}$")]
        [JsonPropertyName("fullName")]
        public string FullName { get; set; }
        [JsonPropertyName("birthDate")]
        public DateTime BirthDate { get; set; }

        public User ToUser()
        {
            User user = new User();
            user.Email = Email;
            user.Password = BCryptHelper.HashPassword(Password, "$2a$10$llw0G6IyibUob8h5XRt9xuRczaGdCm/AiV6SSjf5v78XS824EGbh.");
            user.Info = Info;
            user.FullName = FullName;
            user.BirthDate = BirthDate;
            user.RoleId = 2;

            return user;
        }
    }
}
