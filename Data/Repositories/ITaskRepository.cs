using project_managment.Services;
using Task = pm.Models.Task;

namespace project_managment.Data.Repositories
{
    public interface ITaskRepository : ICrudRepository<Task>
    {

    }
}
