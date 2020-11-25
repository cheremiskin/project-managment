using System.Collections.Generic;
using System.Threading.Tasks;
using pm.Models;

namespace project_managment.Data.Repositories
{
    public interface IUserRepository : ICrudRepository<User>
    { 
        Task<User> FindUserByEmail(string email);
        Task<string> FindRoleByUserId(long id);
        Task<IEnumerable<User>> FindAllUsersInProject(long projectId);
        Task<IEnumerable<User>> FindAllUsersInTask(long taskId);
    }
}
