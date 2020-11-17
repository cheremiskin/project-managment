using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Filters;
using project_managment.Forms;
using project_managment.Services;

namespace project_managment.Controllers
{
    [ApiController]
    [Route("api/projects")]
    public class ProjectController : ControllerBase
    {
        private readonly IProjectRepository projectRepository;
        public ProjectController(IProjectRepository projectRepository)
        {
            this.projectRepository = projectRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Project>>> FindAllProjects()
        {
            var projects = await projectRepository.FindAll();
            return Ok(projects);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Project>> FindUserById(long id)
        {
            Project project = await projectRepository.FindById(id);
            if (project == null)
                return NotFound();
            return Ok(project);
        }

        [HttpDelete]
        [Route("{id}")]
        public async System.Threading.Tasks.Task RemoveProjectById(long id)
        {
            await projectRepository.RemoveById(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async System.Threading.Tasks.Task UpdateProject(Project project)
        {
            await projectRepository.Update(project);
        }

        [HttpPost]
        [Route("create")]
        [ValidateModel]
        public async Task<ActionResult> CreateProject(CreateProjectForm form)
        {
            long creatorId = 1;
            Project project = form.ToProject(creatorId);

            try
            {
                await projectRepository.Save(project);
                return Ok("nice");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}
