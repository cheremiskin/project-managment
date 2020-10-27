using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pm.Models
{
    public class Project
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int CreatorId { get; set; }

        /*----------------------------------------*/
        public List<User> Executors { get; set; }
        public User Creator { get; set; }
        public List<Task> Tasks { get; set; }
    }
}
