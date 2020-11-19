using System.Linq;
using Microsoft.AspNetCore.Mvc;
using project_managment.Data.Repositories;

namespace project_managment.Controllers
{
    
    [ApiController]
    [Route("/api/tasks")]
    public class TaskController : ControllerBase
    {
        private ITaskRepository _taskRepository;

        public TaskController(ITaskRepository taskRepository)
        {
            _taskRepository = taskRepository;
        }
        
    }
}