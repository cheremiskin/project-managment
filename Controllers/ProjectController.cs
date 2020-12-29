using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using pm.Models.UpdateModels;
using project_managment.Data.Models.Dto;
using project_managment.Data.Repositories;
using project_managment.Exceptions;
using project_managment.Filters;
using project_managment.Forms;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize(Policy = "IsUserOrAdmin")]
    public class ProjectController : ControllerBaseExt
    {

        private readonly IUserRepository _userRepository;
        private readonly IProjectRepository _projectRepository;
        public ProjectController(IProjectRepository projectRepository, IUserRepository userRepository, ITaskRepository taskRepository, ICommentRepository commentRepository) : 
                base(userRepository, projectRepository, taskRepository, commentRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FindAllProjects([FromQuery(Name = "page")] int page = 0, 
                                                            [FromQuery(Name = "size")] int size = 10)
        {
            var claim = GetClientRoleClaim();
            IEnumerable<Project> projects = null;
            if (claim == null || claim.Value == Role.RoleUser)
                projects = await _projectRepository.FindAllNotPrivate(page, size);
            if (claim != null && claim.Value == Role.RoleAdmin)
                projects = await _projectRepository.FindAll(page, size);
            
            return Ok(projects);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> FindProjectById(long id)
        {
            var accessLevel = await GetAccessLevelForProject(id);
            switch (accessLevel)
            {
               case AccessLevel.Admin: case AccessLevel.Creator: case AccessLevel.Member: case AccessLevel.Anonymous:
                   return Ok(Cache.Project ?? await _projectRepository.FindById(id));
               default:
                   throw ProjectException.AccessDenied();
            }
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }

        [HttpDelete]
        [Route("{id}")]
        public async Task<IActionResult> RemoveProject(long id)
        {
            var accessLevel = await GetAccessLevelForProject(id);
            switch (accessLevel)
            {
                case AccessLevel.Admin: case AccessLevel.Creator:
                    await _projectRepository.RemoveById(id);
                    return NoContent();
                default:
                    throw ProjectException.AccessDenied();
            }
        }

        [HttpPost]
        [ValidateModel]
        public IActionResult CreateProject(CreateProjectForm form)
        {
            var project = form.ToProject();
            var user = GetClientUser();

            project.CreatorId = user.Id;

            long id =  _projectRepository.Save(project).Result;
            _projectRepository.LinkUserAndProjectById(user.Id, id);

            return Created($"/api/projects/{id}", new {id = id}); // should return id
        }
        
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProject(long id, [FromBody]ProjectUpdate project)
        {
            var accessLevel = await GetAccessLevelForProject(id);
            switch (accessLevel)
            {
                case AccessLevel.Admin: case AccessLevel.Creator:
                    await _projectRepository.Update(project.ToProject(id));
                    return NoContent();
                default:
                    throw ProjectException.UpdateDenied();
            }
        }
        
        
        [HttpGet]
        [Route("{projectId}/users")]
        [AllowAnonymous]
        public async Task<IActionResult> GetMembersOfProject([Required, FromRoute(Name = "projectId")] long projectId)
        {
            var accessLevel = await GetAccessLevelForProject(projectId);
            
            switch (accessLevel)
            {
                case AccessLevel.Admin: case AccessLevel.Member: case AccessLevel.Creator: case AccessLevel.Anonymous:
                    return Ok((Cache.ProjectMembers ?? await _userRepository.FindAllUsersInProject(projectId)).Select(u => new UserDto(u)));
                default:
                    throw ProjectException.AccessDenied();
            } 
        }
        
        [HttpPost]
        [Route("{projectId}/users")]
        public async Task<IActionResult> PostAssignUserToProject([Required, FromRoute(Name = "projectId")] long projectId, 
                                                    [Required, FromQuery(Name = "userId")] long userId)
        {
            var accessLevel = await GetAccessLevelForProject(projectId);
            switch (accessLevel)
            {
                case AccessLevel.Admin: case AccessLevel.Creator:
                    var link = _projectRepository.LinkUserAndProjectById(userId, projectId).Result;
                    if (link == null)
                        throw ProjectException.UserProjectLinkCreationFailed();
                    return Created("", link);
                default:
                    throw ProjectException.AccessDenied();
            }
            
        }

        [HttpDelete]
        [Route("{projectId}/users")]
        
        public async Task<IActionResult> DeleteLinkUserProject([Required, FromRoute(Name = "projectId")] long projectId,
                                            [Required, FromQuery(Name = "userId")] long userId)
        {
            var accessLevel = await GetAccessLevelForProject(projectId);
            switch (accessLevel)
            {
                case AccessLevel.Admin: case AccessLevel.Creator:
                    bool result = await _projectRepository.UnlinkUserAndProjectById(userId, projectId);
                    if (!result)
                        throw ProjectException.UserProjectLinkDeletionFailed();
                    return NoContent();
                default:
                    throw ProjectException.AccessDenied();
            }
        }
    }
}
