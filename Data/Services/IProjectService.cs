using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Threading.Tasks;
using pm.Models;
using Task = System.Threading.Tasks.Task;

namespace project_managment.Data.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<Project>> FindAll();
        Task<IEnumerable<Project>> FindAll(int page, int size);
        Task<Project> FindById(long id);
        Task Remove(Project entity);
        Task RemoveById(long id);
        Task<long> Save(Project entity);
        Task Update(Project entity);
        IEnumerable<Project> FindAllNotPrivate(int page, int size);
        Task AddTaskToProject(Project project, pm.Models.Task task);
        Task AddUserToProject(long project, long user);
        Task RemoveTaskFromProject(Project project, pm.Models.Task task);
        Task RemoveUserFromProject(Project project, User user);
    }
}