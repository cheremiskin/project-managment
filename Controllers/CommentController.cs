using Microsoft.AspNetCore.Mvc;
using project_managment.Data.Services;

namespace project_managment.Controllers
{
    [Route("api/comments")]
    [ApiController]

    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService ;
        public CommentController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        
    }
}