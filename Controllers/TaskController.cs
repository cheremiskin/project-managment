using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using pm.Models;
using project_managment.Data.Repositories;
using project_managment.Exceptions;
using project_managment.Filters;
using project_managment.Forms;

namespace project_managment.Controllers
{
    
    [ApiController]
    [Route("/api/tasks")]
    [Authorize(Policy = "IsUserOrAdmin")]
    public class TaskController : ControllerBaseExt
    {
        private readonly ITaskRepository _taskRepository;
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;

        public TaskController(ITaskRepository taskRepository,IProjectRepository projectRepository, IUserRepository userRepository) : base(userRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        [HttpGet]
        public IActionResult GetTasks([Required, FromQuery(Name = "projectId")] long projectId)
        {
            var project = _projectRepository.FindById(projectId).Result;
            if (project == null)
                return BadRequest(ProjectException.NotFound());

            
            var client = GetClientUser();

            var tasks = _taskRepository.FindAllInProjectById(projectId).Result;
            if (!project.IsPrivate)
                return Ok(tasks);

            var members = _userRepository.FindAllUsersInProject(projectId).Result;
            if (members.FirstOrDefault(u => u.Id == client.Id) != null)
                return Ok(tasks);

            return Unauthorized();
        }

        [HttpGet]
        [Route("{id}")]
        public IActionResult GetTask(long id)
        {
            var task = _taskRepository.FindById(id).Result;
            if (task == null)
                return NotFound(TaskException.NotFound());

            Project parentProject = _projectRepository.FindById(task.ProjectId).Result;

            var role = GetClientRoleClaim()?.Value;
            if (!parentProject.IsPrivate || role == Role.RoleAdmin)
                return Ok(task);
            
            var client = GetClientUser();
            var members = _userRepository.FindAllUsersInProject(parentProject.Id).Result;
            if (members.FirstOrDefault(u => u.Id == client.Id) != null)
                return Ok(task);

            return Unauthorized(ProjectException.AccessDenied());
        }

        [HttpPost]
        [ValidateModel]
        public IActionResult PostTask([FromBody]CreateTaskForm form, [Required, FromQuery(Name = "projectId")] long projectId)
        {
             var project = _projectRepository.FindById(projectId).Result;
            if (project == null)
                return NotFound(ProjectException.NotFound());
            
            var client = GetClientUser();
            var role = GetClientRoleClaim()?.Value;
            if (project.CreatorId != client.Id && role != Role.RoleAdmin)
                return Unauthorized(ProjectException.AccessDenied());
            
            var task = form.ToTask();
            task.ProjectId = projectId;

            var id = _taskRepository.Save(task).Result;
            if (id == 0)
                return BadRequest(TaskException.PostFailed());
            return Created($"/api/tasks/{id}", id);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult DeleteTask(long id)
        {
            throw new NotImplementedException(); 
        }
        
        
    }
}