using System.Collections.Generic;
using System.Threading.Tasks;
using pm.Models;
using project_managment.Services;
using Task = System.Threading.Tasks.Task;

namespace project_managment.Data.Repositories
{
    public interface IProjectRepository : ICrudRepository<Project>
    {
        Task<IEnumerable<Project>> FindProjectsByName(string name);
        Task<IEnumerable<Project>> FindNotPrivateProjects();
        Task LinkUserAndProject(User user, Project project);
    }
}
