using Microsoft.AspNetCore.Mvc;
using project_managment.Data.Repositories;

namespace project_managment.Controllers
{
    [Route("api/comments")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository ;
        public CommentController(ICommentRepository commentRepository)
        {
            _commentRepository = commentRepository;
        }

        
    }
}