using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace pm.Models
{
    public class Role
    {
        public long Id { get; set; }
        public string Name { get; set; }
        
        [JsonIgnore]
        public const string RoleAdmin = "ROLE_ADMIN";
        [JsonIgnore] 
        public const string RoleUser = "ROLE_USER";

    }
}
