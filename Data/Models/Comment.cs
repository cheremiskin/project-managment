using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace pm.Models
{
    public class Comment
    {
        public long Id { get; set; }
        public string Content { get; set; }
        public int UserId { get; set; }
        public int TaskId { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
