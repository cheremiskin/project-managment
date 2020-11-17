using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using pm.Models;
using project_managment.Filters;
using project_managment.Forms;
using project_managment.Services;

namespace project_managment.Controllers
{
    [Route("api/comments")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository commentRepository;
        public CommentController(ICommentRepository commentRepository)
        {
            this.commentRepository = commentRepository;
        }

        [HttpGet]
        [Authorize]

        public async Task<ActionResult<IEnumerable<Comment>>> FindAllComments()
        {
            var comments = await commentRepository.FindAll();
            return Ok(comments);
        }

        [HttpGet]
        [Route("{id}")]
        public async Task<ActionResult<Comment>> FindCommentById(long id)
        {
            Project comment = await commentRepository.FindById(id);
            if (comment == null)
                return NotFound();
            return Ok(comment);
        }

        [HttpDelete]
        [Route("{id}")]
        public async System.Threading.Tasks.Task RemoveCommentById(long id)
        {
            await commentRepository.RemoveById(id);
        }

        [HttpPut]
        [Route("{id}")]
        public async System.Threading.Tasks.Task UpdateComment(Comment comment)
        {
            await commentRepository.Update(comment);
        }

        [HttpPost]
        [Route("create")]
        [ValidateModel]
        public async Task<ActionResult> CreateComment(CreateCommentForm form)
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