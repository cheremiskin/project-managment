using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Data.Repositories.RepositoryImpl;
using project_managment.Data.Services;
using project_managment.Filters;
using project_managment.Forms;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize(Policy = "IsUserOrAdmin")]
    public class ProjectController : ControllerBase
    {

        private readonly IUserService _userService;
        private readonly IProjectService _projectService;
        public ProjectController(IProjectService projectService, IUserService userService)
        {
            _projectService = projectService;
            _userService = userService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> FindAllProjects([Required,FromQuery(Name = "page")] int page, 
                                                            [Required, FromQuery(Name = "size")] int size)
        {
            var claim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            IEnumerable<Project> projects = null;
            if (claim == null || claim.Value == "ROLE_USER")
                projects = _projectService.FindAllNotPrivate(page, size);
            if (claim != null && claim.Value == "ROLE_ADMIN")
                projects = await _projectService.FindAll(page, size);
            
            return Ok(projects);
        }

        [HttpGet]
        [Route("{id}")]
        [AllowAnonymous]
        public IActionResult FindProjectById(long id)
        {
            var project =  _projectService.FindById(id).Result;
            if (project == null)
                return NotFound();
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            if (role == "ROLE_ADMIN")
            {
                return Ok(project);
            }

            if (role == "ROLE_USER")
            {
                if (project.IsPrivate)
                {
                    IEnumerable<User> members = _userService.FindAllUsersInProject(project);
                    if (members.FirstOrDefault(u => u.Email == User.Identity.Name) == null)
                        return Forbid();
                }
                return Ok(project);
            }
            
            if (role == null)
            {
                if (project.IsPrivate)
                    return Forbid();
                return Ok(project);
            }
            
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }

        [HttpDelete]
        [Route("{id}")]
        public IActionResult RemoveProject(long id)
        {
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            var project = _projectService.FindById(id).Result;
            if (project == null)
                return NotFound("project not found");
            
            if (role == "ROLE_ADMIN")
            {
                _projectService.Remove(project);
                return Ok();
            }

            if (role == "ROLE_USER")
            {
                var user = _userService.FindByEmail(User.Identity.Name);
                if (user.Id != project.CreatorId)
                    return Unauthorized("you can't delete this project");
                _projectService.Remove(project);
                return Ok();
            }

            return StatusCode((int) HttpStatusCode.InternalServerError);
        }

        [HttpPost]
        [ValidateModel]
        public IActionResult CreateProject(CreateProjectForm form)
        {
            var project = form.ToProject();
            var user = _userService.FindByEmail(User.Identity.Name);

            project.CreatorId = user.Id;

            long id =  _projectService.Save(project).Result;
            project.Id = id;

            return Created($"/api/projects/{id}", project); // should return id
        }
        
        [HttpPut]
        [Route("{id}")]
        public async Task<IActionResult> UpdateProject(long id, Project project)
        {
            var role = User.Claims.FirstOrDefault(u => u.Type == ClaimTypes.Role)?.Value;
            project.Id = id;
            if (role == "ROLE_ADMIN")
            {
                await _projectService.Update(project);
                return Ok();
            }

            User user = _userService.FindByEmail(User.Identity.Name);
            if (role == "ROLE_USER")
            {
                Project targetProject = _projectService.FindById(id).Result;
                if (targetProject == null)
                    return BadRequest("project was not found");
                if (targetProject.CreatorId != user.Id)
                    return Unauthorized();

                await _projectService.Update(project);
                return Ok();
            }

            return StatusCode((int) HttpStatusCode.InternalServerError);
        }


        [HttpPost]
        [Route("{id}/users")]
        public IActionResult AssignUserToProject(long id, [FromQuery(Name = "userId")] long userId)
        {
            throw new NotImplementedException();
        }
        

    }
}
