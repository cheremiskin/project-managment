using System.Collections.Generic;
using System.Threading.Tasks;
using pm.Models;

namespace project_managment.Data.Repositories
{
    public interface IUserRepository : ICrudRepository<User>
    { 
        Task<User> FindUserByEmail(string email);
        Task<string> FindRoleByUserId(long id);
        Task<IEnumerable<User>> FindUsersInProject(long projectId);
    }
}
