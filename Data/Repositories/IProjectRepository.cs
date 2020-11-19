using System.Collections.Generic;
using System.Threading.Tasks;
using pm.Models;
using pm.Models.Links;
using Task = System.Threading.Tasks.Task;

namespace project_managment.Data.Repositories
{
    public interface IProjectRepository : ICrudRepository<Project>
    {
        Task<IEnumerable<Project>> FindProjectsByName(string name);
        Task<IEnumerable<Project>> FindAllNotPrivate(int page, int size);
        Task<ProjectUser> LinkUserAndProject(User user, Project project);
        Task<ProjectUser> LinkUserAndProjectById(long userId, long projectId);
        Task<bool> UnlinkUserAndProjectById(long userId, long projectId);
    }
}
