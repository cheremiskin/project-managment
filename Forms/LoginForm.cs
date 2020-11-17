using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace project_managment.Forms
{
    public class LoginForm
    {
        [Required]
        [EmailAddress]
        [JsonPropertyName("email")]
        public string Email { get; set; }
        
        [Required]
        [JsonPropertyName("password")]
        public string Password { get; set; }

        public bool PasswordMatch(string userPassword, Func<string, string, bool> checkPassword)
        {
            return checkPassword(Password, userPassword);
        }
    }
}
