using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers.Cache;
using pm.Models;
using project_managment.Data.Repositories;
using project_managment.Exceptions;
using project_managment.Filters;
using project_managment.Pages;
using Task = pm.Models.Task;

namespace project_managment.Controllers
{
    [ExceptionFilter]
    public class ControllerBaseExt : ControllerBase
    {
        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ITaskRepository _taskRepository;
        private readonly ICommentRepository _commentRepository;
        protected CacheAccumulator Cache { get; } = new CacheAccumulator();

        public ControllerBaseExt(IUserRepository userRepository, IProjectRepository projectRepository, 
                                ITaskRepository taskRepository, ICommentRepository commentRepository)
        {
            _userRepository = userRepository;
            _projectRepository = projectRepository;
            _taskRepository = taskRepository;
            _commentRepository = commentRepository;
        }
        
        protected User GetClientUser()
        {
            var email = User.Identity.Name;
            return email == null ? null : _userRepository.FindUserByEmail(email).Result;
        }

        protected Claim GetClientRoleClaim()
        {
            return User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
        }

        protected long GetClientId()
        {
            long.TryParse(User.Claims.FirstOrDefault(c => c.Type == "UserId")?.Value, out var id);
            return id;
        }

        protected async Task<AccessLevel> GetAccessLevelForProject(long projectId)
        {
            var project = await _projectRepository.FindById(projectId);
            if (project == null)
                throw ProjectException.NotFound();

            Cache.Project = project;
            
            var role = GetClientRoleClaim()?.Value;
            if (role == Role.RoleAdmin)
                return AccessLevel.Admin;

            var userId = GetClientId();

            if (project.CreatorId == userId)
                return AccessLevel.Creator;

            var members = await _userRepository.FindAllUsersInProject(projectId);
            Cache.ProjectMembers = members;
            if (members.FirstOrDefault(m => m.Id == userId) != null)
                return AccessLevel.Member;

            return (!project.IsPrivate?.Equals(true) ?? true )  ? AccessLevel.Anonymous : AccessLevel.None;
            
        }

        protected async Task<AccessLevel> GetAccessLevelForTask(long taskId)
        {
            var task = await _taskRepository.FindById(taskId);
            if (task == null)
                throw TaskException.NotFound();
            Cache.Task = task;
            return await GetAccessLevelForProject(task.ProjectId);
        }

        protected async Task<AccessLevel> GetAccessLevelForComment(long commentId)
        {
            var comment = await _commentRepository.FindById(commentId);
            if (comment == null)
                throw CommentException.NotFound();
            Cache.Comment = comment;

            var userId = GetClientId();

            if (comment.UserId == userId)
                return AccessLevel.Creator;
            
            return await GetAccessLevelForTask(comment.TaskId);
        }

        public enum AccessLevel
        {
            Member, Creator, Admin, Anonymous, None
        }

        protected class CacheAccumulator
        {
            public User Client { get; set; }
            public Project Project { get; set; }
            public Task Task { get; set; }
            public Comment Comment { get; set; }
            public IEnumerable<User> ProjectMembers { get; set; }
            public IEnumerable<User> TaskMembers { get; set; }
        }


    }
}