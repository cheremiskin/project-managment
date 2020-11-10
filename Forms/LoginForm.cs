using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_managment.Forms
{
    public class LoginForm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        
        [Required]
        public string Password { get; set; }

        public bool PasswordMatch(string userPassword, Func<string, string, bool> checkPassword)
        {
            return checkPassword(Password, userPassword);
        }
    }
}
