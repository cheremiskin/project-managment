using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;
using Microsoft.IdentityModel.Tokens;
using pm.Models;
using pm.Models.Links;
using pm.Models.UpdateModels;
using project_managment.Data.Models.Dto;
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

        public TaskController(ITaskRepository taskRepository,IProjectRepository projectRepository, IUserRepository userRepository, ICommentRepository commentRepository) : 
            base(userRepository, projectRepository, taskRepository, commentRepository)
        {
            _taskRepository = taskRepository;
            _userRepository = userRepository;
            _projectRepository = projectRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetTasks([Required, FromQuery(Name = "projectId")] long projectId)
        {
            var accessLevel = await GetAccessLevelForProject(projectId);
            switch (accessLevel)
            {
                case AccessLevel.Member: case AccessLevel.Creator: case AccessLevel.Admin: case AccessLevel.Anonymous:
                    var tasks = await _taskRepository.FindAllInProjectById(projectId);
                    return Ok(tasks);
                default:
                    throw ProjectException.AccessDenied();
            }
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<IActionResult> GetTask(long id)
        {
            var accessLevel = await GetAccessLevelForTask(id);
            switch (accessLevel)
            {
                case AccessLevel.Member: case AccessLevel.Creator: case AccessLevel.Admin: case AccessLevel.Anonymous:
                    return Ok(Cache.Task ?? await _taskRepository.FindById(id));
                default: 
                    throw TaskException.AccessDenied();
            }
        }

        [HttpPost]
        [ValidateModel]
        public async Task<IActionResult> PostTask([FromBody]CreateTaskForm form, [Required, FromQuery(Name = "projectId")] long projectId)
        {
            var accessLevel = await GetAccessLevelForProject(projectId);
            switch (accessLevel)
            {
                case AccessLevel.Creator: case AccessLevel.Admin:
                    var task = form.ToTask();
                    task.ProjectId = projectId;
                    var id = await _taskRepository.Save(task);
                    if (id == 0)
                        throw TaskException.PostFailed();
                    return Created($"/api/tasks/{id}", await _taskRepository.FindById(id));
                default:
                    throw ProjectException.AccessDenied();
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteTask(long id)
        {
            var accessLevel = await GetAccessLevelForTask(id);
            switch (accessLevel)
            {
                case AccessLevel.Creator: case AccessLevel.Admin:
                    await _taskRepository.RemoveById(id);
                    return NoContent();
                default:
                    throw TaskException.AccessDenied();
            }
        }

        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> PutTask(long id, [FromBody] TaskUpdate task)
        {
            var accessLevel = await GetAccessLevelForTask(id);
            switch (accessLevel)
            {
                case AccessLevel.Creator: case AccessLevel.Admin:
                    try
                    {
                        await _taskRepository.Update(task.ToTask(id));
                    }
                    catch (Exception ex)
                    {
                        throw TaskException.UpdateFail();
                    }
                    return Ok();
                default:
                    throw TaskException.AccessDenied();
            }
        }
        
        
        [HttpGet]
        [Route("{id}/users")]
        public async Task<IActionResult> GetTaskUsers(long id)
        {
            var accessLevel = await GetAccessLevelForTask(id);
            switch (accessLevel)
            {
                case AccessLevel.Member: case AccessLevel.Creator: case AccessLevel.Admin: case AccessLevel.Anonymous:
                    var users = await _userRepository.FindAllUsersInTask(id);
                    return Ok(users.Select(u => new UserDto(u)));
                default:
                    throw ProjectException.AccessDenied();
            }
            
        }

        [HttpPost]
        [Route("{taskId}/users")]
        public async Task<IActionResult> PostTaskUser([FromRoute(Name = "taskId")] long taskId,
            [FromQuery(Name = "userId"), Required] long userId)
        {
            var accessLevel = await GetAccessLevelForTask(taskId);
            switch (accessLevel)
            {
                case AccessLevel.Creator: case AccessLevel.Admin:
                    var userProjectLink = await _projectRepository.FindLink(userId, Cache.Project.Id);

                    if (userProjectLink == null)
                        throw ProjectException.AccessDenied();
                    
                    var link = await _taskRepository.LinkUserAndTask(userId, taskId);
                    if (link == null)
                        throw TaskException.LinkFailed();
                    return Created("", link);
                default:
                    throw ProjectException.AccessDenied();
            }
        }

        [HttpDelete]
        [Route("{taskId}/users")]

        public async Task<IActionResult> DeleteTaskUser([FromRoute(Name = "taskId")] long taskId,
            [FromQuery(Name = "userId"), Required] long userId)
        {
            var accessLevel = await GetAccessLevelForTask(taskId);
            switch (accessLevel)
            {
                case AccessLevel.Creator: case AccessLevel.Admin:
                    await _taskRepository.UnlinkUserAndTask(userId, taskId);
                    return NoContent(); 
                default:
                    throw ProjectException.AccessDenied();
            }
        }

        [HttpGet]
        [Route("statuses")]
        [AllowAnonymous]
        public async Task<IActionResult> GetStatuses()
        {
            return Ok(await _taskRepository.FindAllStatuses());
        }

    }
}