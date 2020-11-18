using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using pm.Models;
using project_managment.Data.Repositories;
using project_managment.Data.Services;
using project_managment.Filters;
using project_managment.Forms;
using project_managment.Services;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize(Policy = "IsAdmin")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IProjectService _projectService;
        public ProjectController(IProjectRepository projectRepository, IProjectService projectService)
        {
            _projectRepository = projectRepository;
            _projectService = projectService;
        }

        [HttpGet]
        [Authorize(Policy = "IsUserOrAdmin")]
        public async Task<IActionResult> FindAllProjects([Required,FromQuery(Name = "page")] int page, 
                                                            [Required, FromQuery(Name = "size")] int size)
        {
            var projects = await _projectService.FindAll(page, size);
            return Ok(projects);
        }

        [HttpGet]
        [Route("{id}")]
        [Authorize(Policy = "IsUserOrAdmin")]
        public async Task<ActionResult<Project>> FindProjectById(long id)
        {
            Project project = await _projectRepository.FindById(id);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpDelete]
        [Route("{id}")]
        public async System.Threading.Tasks.Task RemoveProjectById(long id)
        {
            await _projectRepository.RemoveById(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async System.Threading.Tasks.Task UpdateProject(Project project)
        {
            await _projectRepository.Update(project);
        }

        [HttpPost]
        [Route("create")]
        [ValidateModel]
        public async Task<ActionResult> CreateProject(CreateProjectForm form)
        {
            long creatorId = 1;
            Project project = form.ToProject();

            try
            {
                await _projectRepository.Save(project);
                return Ok("nice");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost]
        [Route("{id}/users")]
        // [Authorize(Roles = "ROLE_ADMIN")]
        public IActionResult AssignUserToProject(long id, [FromQuery(Name = "userId")] long userId)
        {
            _projectService.AddUserToProject(id, userId, User.Identity);
            throw new NotImplementedException();
        }
        

    }
}
