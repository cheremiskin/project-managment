using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pm.Models
{
    public class Task
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public int StatusId { get; set; }
        public string Content { get; set; }
        public long ProjectId { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public DateTime? ExecutionTime { get; set; }

    }
}
