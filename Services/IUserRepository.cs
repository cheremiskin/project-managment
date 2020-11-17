using pm.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task = pm.Models.Task;

namespace project_managment.Services
{
    public interface IUserRepository : ICrudRepository<User>
    { 
        Task<User> FindUserByEmail(string email);
        Task<string> FindRoleByUserId(long id);
        Task<bool> EmailExists(string email); // метод не совсем подходит по логике к репозиторию, нужно с этим чот придумать
                                              // можно сделать что то типа UserService 
    }
}
