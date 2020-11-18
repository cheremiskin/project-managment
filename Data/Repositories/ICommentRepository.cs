using System.Collections.Generic;
using System.Threading.Tasks;
using pm.Models;

namespace project_managment.Data.Repositories
{
    public interface ICommentRepository : ICrudRepository<Comment>
    {
        // Task<IEnumerable<Comment>> FindProjectsByName(string Name);
        Task<IEnumerable<Comment>> FindCommentsByTaskId(long taskId);
    }
}