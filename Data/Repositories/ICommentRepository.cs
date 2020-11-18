using pm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_managment.Services
{
    public interface ICommentRepository : ICrudRepository<Comment>
    {
        // Task<IEnumerable<Comment>> FindProjectsByName(string Name);
        Task<IEnumerable<Comment>> FindCommentsByTaskId(long taskId);
    }
}