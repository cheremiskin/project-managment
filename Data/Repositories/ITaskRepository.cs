using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using pm.Models.Links;
using Task = pm.Models.Task;

namespace project_managment.Data.Repositories
{
    public interface ITaskRepository : ICrudRepository<Task>
    {
        Task<IEnumerable<Task>> FindAllInProjectById(long projectId);
        Task<TaskUser> LinkUserAndTask(long userId, long taskId);
        Task<bool> UnlinkUserAndTask(long userId, long taskId);
        Task<bool> UnlinkAllUsersFromTask(long taskId);

        Task<IEnumerable<Status>> FindAllStatuses();
    }
}
