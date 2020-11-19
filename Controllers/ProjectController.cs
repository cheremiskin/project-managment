using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Data.Dto;
using project_managment.Data.Repositories;
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
        public ProjectController(IProjectRepository projectRepository, IUserRepository userRepository) : base(userRepository)
        {
            _projectRepository = projectRepository;
            _userRepository = userRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FindAllProjects([Required,FromQuery(Name = "page")] int page, 
                                                            [Required, FromQuery(Name = "size")] int size)
        {
            var claim = GetClientRoleClaim();
            IEnumerable<Project> projects = null;
            if (claim == null || claim.Value == "ROLE_USER")
                projects = await _projectRepository.FindAllNotPrivate(page, size);
            if (claim != null && claim.Value == "ROLE_ADMIN")
                projects = await _projectRepository.FindAll(page, size);
            
            return Ok(projects);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public IActionResult FindProjectById(long id)
        {
            var project =  _projectRepository.FindById(id).Result;
            if (project == null)
                return NotFound();
            var role = GetClientRoleClaim()?.Value;
            switch (role)
            {
                case "ROLE_ADMIN":
                    return Ok(project);
                case "ROLE_USER" when !project.IsPrivate:
                    return Ok(project);
                case "ROLE_USER":
                {
                    var members = _userRepository.FindAllUsersInProject(project.Id).Result;
                    if (members.FirstOrDefault(u => u.Email == User.Identity.Name) == null)
                        return Forbid();
                    return Ok(project);
                }
                case null when project.IsPrivate:
                    return Forbid();
                case null:
                    return Ok(project);
                default:
                    return StatusCode((int) HttpStatusCode.InternalServerError);
            }
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult RemoveProject(long id)
        {
            var role = GetClientRoleClaim()?.Value;
            var project = _projectRepository.FindById(id).Result;
            if (project == null)
                return NotFound("project not found");
            
            if (role == "ROLE_ADMIN")
            {
                _projectRepository.Remove(project);
                return Ok();
            }

            if (role == "ROLE_USER")
            {
                var user = _userRepository.FindUserByEmail(User.Identity.Name).Result;
                if (user.Id != project.CreatorId)
                    return Unauthorized("you can't delete this project");
                _projectRepository.Remove(project);
                return Ok();
            }

            return StatusCode((int) HttpStatusCode.InternalServerError);
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
            project.Id = id;

            return Created($"/api/projects/{id}", project); // should return id
        }
        
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProject(long id, Project project)
        {
            var role = GetClientRoleClaim()?.Value;
            project.Id = id;
            if (role == "ROLE_ADMIN")
            {
                await _projectRepository.Update(project);
                return Ok();
            }

            var user = GetClientUser();
            if (role == "ROLE_USER")
            {
                var targetProject = _projectRepository.FindById(id).Result;
                if (targetProject == null)
                    return BadRequest("project was not found");
                if (targetProject.CreatorId != user.Id)
                    return Unauthorized();

                await _projectRepository.Update(project);
                return Ok();
            }

            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
        
        
        [HttpGet]
        [Route("{projectId}/users")]
        public IActionResult GetMembersOfProject([Required, FromRoute(Name = "projectId")] long projectId)
        {
            var project = _projectRepository.FindById(projectId).Result;

            if (project == null)
                return NotFound();
            var client = GetClientUser();
            var role = GetClientRoleClaim()?.Value;

            var members = _userRepository.FindAllUsersInProject(projectId).Result.Select(u => new UserDto(u));

            if (!project.IsPrivate || role == Role.RoleAdmin)
                return Ok(members);

            if (members.FirstOrDefault(u => u.Id == client.Id) != null)
                return Ok(members);

            return Unauthorized();
        }
        
        [HttpPost]
        [Route("{projectId}/users")]
        public IActionResult PostAssignUserToProject([Required, FromRoute(Name = "projectId")] long projectId, 
                                                    [Required, FromQuery(Name = "userId")] long userId)
        {
            var project = _projectRepository.FindById(projectId).Result;
            if (project == null)
                return NotFound("project was not found");
            var role = GetClientRoleClaim()?.Value;
            var client = GetClientUser();
            if (role != Role.RoleAdmin && project.CreatorId != client.Id)
                return Unauthorized("you don't have rights to perform this operation");

            var link = _projectRepository.LinkUserAndProjectById(userId, projectId).Result;
            return link == null ? (IActionResult) BadRequest() : Created("", link);
        }

        [HttpDelete]
        [Route("{projectId}/users")]
        public IActionResult DeleteLinkUserProject([Required, FromRoute(Name = "projectId")] long projectId,
                                            [Required, FromQuery(Name = "userId")] long userId)
        {
            var project = _projectRepository.FindById(projectId).Result;
            if (project == null)
                return NotFound("project was not found");
            var role = GetClientRoleClaim()?.Value;
            var client = GetClientUser();
            if (role != Role.RoleAdmin && project.CreatorId != client.Id)
                return Unauthorized("you don't have rights to perform this operation");

            bool result = _projectRepository.UnlinkUserAndProjectById(userId, projectId).Result;
            return result ? (IActionResult) NoContent() : BadRequest();
        }
    }
}
