using DevOne.Security.Cryptography.BCrypt;
using pm.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace project_managment.Forms
{
    public class RegistrationForm
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        [Required]
        public string PasswordConfirm { get; set; }
        public string Info { get; set; }
        [StringLength(128)]
        [Required]
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }

        public User ToUser()
        {
            User user = new User();
            user.Email = Email;
            user.Password = BCryptHelper.HashPassword(Password, "$2a$10$llw0G6IyibUob8h5XRt9xuRczaGdCm/AiV6SSjf5v78XS824EGbh.");
            user.Info = Info;
            user.FullName = FullName;
            user.BirthDate = BirthDate;

            return user;
        }
    }
}
