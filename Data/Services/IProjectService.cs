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
        Task<Project> FindById(long id, IIdentity identity);
        Task Remove(Project entity, IIdentity identity );
        Task RemoveById(long id, IIdentity identity);
        Task Save(Project entity, IIdentity identity);
        Task Update(Project entity, IIdentity identity);
        Task<IEnumerable<Project>> FindAllNotPrivate(int page, int size);
        Task AddTaskToProject(Project project, pm.Models.Task task, IIdentity identity );
        Task AddUserToProject(long project, long user, IIdentity identity);
        Task RemoveTaskFromProject(Project project, pm.Models.Task task, IIdentity identity);
        Task RemoveUserFromProject(Project project, User user, IIdentity identity);
    }
}