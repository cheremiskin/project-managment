using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Data.Repositories;
using project_managment.Exceptions;
using project_managment.Filters;
using project_managment.Forms;

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
            IProjectRepository projectRepository, IUserRepository userRepository) :
            base(userRepository, projectRepository, taskRepository, commentRepository)
        {
            _commentRepository = commentRepository;
            _taskRepository = taskRepository;
            _projectRepository = projectRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        [ExceptionFilter]
        public async Task<IActionResult> GetComments([Required, FromQuery(Name = "taskId")] long taskId)
        {
            var accessLevel = await GetAccessLevelForTask(taskId);
            
            switch (accessLevel)
            {
                case AccessLevel.Admin: case AccessLevel.Creator : case AccessLevel.Member: case AccessLevel.Anonymous:
                    var comments = await _commentRepository.FindCommentsByTaskId(taskId);
                    return Ok(comments);
                case AccessLevel.None:
                    throw CommentException.AccessDenied();
            }
            
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
        

        [HttpPost]
        public async Task<IActionResult> PostComment([Required, FromQuery(Name = "taskId")] long taskId,
                                                        [FromBody] CreateCommentForm form)
        {
            var accessLevel = await GetAccessLevelForTask(taskId);
            
            switch (accessLevel)
            {
                case AccessLevel.Admin: case AccessLevel.Creator: case AccessLevel.Member:
                    var comment = form.ToComment();
                    var userId = GetClientId();
                    comment.TaskId = taskId;
                    comment.UserId = userId;
                    var commentId = await _commentRepository.Save(comment);
                    return Created($"/api/comments/{commentId}", new {id = commentId});
                case AccessLevel.Anonymous: case AccessLevel.None:
                    throw TaskException.AccessDenied();
            }
            
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> DeleteComment([FromRoute(Name = "id")] long id)
        {
            var accessLevel = await GetAccessLevelForComment(id);
            switch (accessLevel)
            {
               case AccessLevel.Admin: case AccessLevel.Creator:
                   await _commentRepository.RemoveById(id);
                   return NoContent();
               case AccessLevel.Anonymous: case AccessLevel.Member: case AccessLevel.None:
                   throw CommentException.AccessDenied();
            }
            
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
        
    }
}