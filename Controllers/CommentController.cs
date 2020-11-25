using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Data.Repositories;
using project_managment.Exceptions;
using project_managment.Forms;
using Task = pm.Models.Task;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/comments")]
    [Authorize(Policy = "IsUserOrAdmin")]
    public class CommentController : ControllerBaseExt
    {
        private readonly ICommentRepository _commentRepository ;
        private readonly ITaskRepository _taskRepository;
        private readonly IProjectRepository _projectRepository;

        public CommentController(ICommentRepository commentRepository, ITaskRepository taskRepository,
            IProjectRepository projectRepository, IUserRepository userRepository) : base(userRepository)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> getComments([Required, FromQuery(Name = "taskId")] long taskId)
        {
            var task = await _taskRepository.FindById(taskId);
            if (task == null)
                return NotFound(TaskException.NotFound());
            var project = await _projectRepository.FindById(task.ProjectId);
            var role = GetClientRoleClaim()?.Value; 

            if (!project.IsPrivate || role == Role.RoleAdmin)
            {
                var comments = await _commentRepository.FindCommentsByTaskId(taskId);
                return Ok(comments);
            }

            var user = GetClientUser();
            if (user == null)
                return Unauthorized(ProjectException.AccessDenied());

            var members = _userRepository.FindAllUsersInProject(project.Id).Result;
            if (members.FirstOrDefault(m => m.Id == user.Id) == null) 
                return Unauthorized(ProjectException.AccessDenied());

            var commentList = await _commentRepository.FindCommentsByTaskId(taskId);
            return Ok(commentList.OrderBy(c => c.CreationDate));
        }

        [HttpPost]
        public async Task<IActionResult> postComment([Required, FromQuery(Name = "taskId")] long taskId,
                                                        [FromBody] CreateCommentForm form)
        {
            var user = GetClientUser();
            var role = GetClientRoleClaim()?.Value;

            var task = await _taskRepository.FindById(taskId);
            if (task == null)
                return NotFound(TaskException.NotFound());

            var project = await _projectRepository.FindById(task.ProjectId);

            var members = await _userRepository.FindAllUsersInProject(project.Id);
            if (members.FirstOrDefault(m => m.Id == user.Id) == null && role != Role.RoleAdmin)
                return Unauthorized(TaskException.AccessDenied());
            
            var comment = form.ToComment();
            comment.UserId = user.Id;
            comment.TaskId = taskId;

            var id = _commentRepository.Save(comment);

            return Created($"/api/comments/{id}", comment);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> deleteComment([FromRoute(Name = "id")] long id)
        {
            var role = GetClientRoleClaim()?.Value;
            if (role == Role.RoleAdmin)
            {
                await _commentRepository.RemoveById(id);
                return NoContent();
            }
            
            var user = GetClientUser();
            var comment = await _commentRepository.FindById(id);

            if (comment == null)
                return NoContent();

            if (comment.UserId != user.Id)
                return Unauthorized();

            await _commentRepository.Remove(comment);
            return NoContent();
        }
        
    }
}