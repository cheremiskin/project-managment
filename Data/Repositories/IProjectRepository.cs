using System.Collections.Generic;
using System.Threading.Tasks;
using pm.Models;
using Task = System.Threading.Tasks.Task;

namespace project_managment.Data.Repositories
{
    public interface IProjectRepository : ICrudRepository<Project>
    {
        Task<IEnumerable<Project>> FindProjectsByName(string name);
        Task<IEnumerable<Project>> FindAllNotPrivate(int page, int size);
        Task LinkUserAndProject(User user, Project project);
    }
}
