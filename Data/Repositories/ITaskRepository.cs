using System.Collections.Generic;
using Task = pm.Models.Task;

namespace project_managment.Data.Repositories
{
    public interface ITaskRepository : ICrudRepository<Task>
    {
        System.Threading.Tasks.Task<IEnumerable<Task>> FindAllInProjectById(long projectId);

    }
}
