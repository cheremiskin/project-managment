using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pm.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; }
        public string FullName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Password { get; set; }
        [JsonIgnore]
        public string Info { get; set; }

        [JsonIgnore]
        public int RoleId { get; set; }
        

        public override string ToString()
        {
            return $"<Id {Id}; Email {Email}>";
        }

        public bool CheckPassword(string password, Func<string, string, bool> checkPassword)
        {
            return checkPassword(Password, password);
        }
    }
}
