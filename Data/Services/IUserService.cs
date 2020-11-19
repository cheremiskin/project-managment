using System.Collections.Generic;
using pm.Models;

namespace project_managment.Data.Services
{
    public interface IUserService
    {
        IEnumerable<User> FindAll();
        IEnumerable<User> FindAll(int page, int size);
        User FindById(long id);
        User FindByEmail(string identityName);
        long Save(User user);
        void Update(User user);
        void Remove(User user);
        void RemoveById(long id);
        
        IEnumerable<User> FindAllUsersInProject(Project project);
        IEnumerable<User> FindAllUsersInProjectById(long projectId);
    }
}