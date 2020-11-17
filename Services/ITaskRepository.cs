using Task = pm.Models.Task;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace project_managment.Services
{
    public interface ITaskRepository : ICrudRepository<Task>
    {

    }
}
