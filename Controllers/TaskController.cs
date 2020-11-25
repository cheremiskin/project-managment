using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using pm.Models;
using pm.Models.Links;
using project_managment.Data.Dto;
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
        public async Task<IActionResult> DeleteTask(long id)
        {
            var client = GetClientUser();
            var role = GetClientRoleClaim()?.Value;
            if (role == Role.RoleAdmin)
            {
                await _taskRepository.UnlinkAllUsersFromTask(id);
                await _taskRepository.RemoveById(id);

                return NoContent();
            }
            var task = await _taskRepository.FindById(id); /* TODO тут от таска мне нужен только ProjectId,
                                                            мб стоит добавить метод
                                                            в репозиторий который будет доставать
                                                            его и сразу искать нужный проект */
            var project = await _projectRepository.FindById(task.ProjectId);

            if (project.CreatorId != client.Id)
                return Unauthorized(TaskException.AccessDenied());

            await _taskRepository.UnlinkAllUsersFromTask(id);
            await _taskRepository.RemoveById(id);

            return NoContent();
        }

        [HttpGet]
        [Route("{id}/users")]
        public IActionResult GetTaskUsers(long id)
        {
            var role = GetClientRoleClaim()?.Value;
            if (role == Role.RoleAdmin)
            {
                var members = _userRepository.FindAllUsersInTask(id).Result; // вероятно стоит проверять существование таска
                return Ok(members.Select(u => new UserDto(u)));
            }

            var task = _taskRepository.FindById(id).Result;
            if (task == null)
                return NotFound(TaskException.NotFound());
            var parentProject = _projectRepository.FindById(task.ProjectId).Result;
            
            var client = GetClientUser();
            var projectMembers = _userRepository.FindAllUsersInProject(parentProject.Id).Result;
            if (projectMembers.FirstOrDefault(u => u.Id == client.Id) != null)
            {
                var taskMembers = _userRepository.FindAllUsersInTask(id).Result;
                return Ok(taskMembers.Select(u => new UserDto(u)));
            }

            return Unauthorized(TaskException.AccessDenied());
        }

        [HttpPost]
        [Route("{taskId}/users")]
        public IActionResult PostTaskUser([FromRoute(Name = "taskId")] long taskId,
            [FromQuery(Name = "userId"), Required] long userId)
        {
            var role = GetClientRoleClaim()?.Value;
            if (role == Role.RoleAdmin)
            {
                var link = _taskRepository.LinkUserAndTask(userId, taskId).Result;
                if (link == null)
                    return NotFound(TaskException.LinkFailed());
                return Created("", link);
            }

            var project = _projectRepository.FindProjectByTaskId(taskId).Result;
            var client = GetClientUser();
            if (project.CreatorId != client.Id)
                return Unauthorized(ProjectException.AccessDenied());

            var link1 = _taskRepository.LinkUserAndTask(userId, taskId).Result;
            if (link1 == null)
                return NotFound(TaskException.LinkFailed());
            return Created("", link1);
        }
        
        
    }
}