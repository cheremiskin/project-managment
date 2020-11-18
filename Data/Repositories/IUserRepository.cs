using System.Threading.Tasks;
using pm.Models;
using project_managment.Services;

namespace project_managment.Data.Repositories
{
    public interface IUserRepository : ICrudRepository<User>
    { 
        Task<User> FindUserByEmail(string email);
        Task<string> FindRoleByUserId(long id);
    }
}
