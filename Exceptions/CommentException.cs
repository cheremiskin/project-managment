using System.Net;
using pm.Models;

namespace project_managment.Exceptions
{
    public class CommentException : BaseException
    {
        public static CommentException NotFound()
        {
            return new CommentException
            {
                Code = "comment_not_found",
                Message = "comment with this id doesn't exist",
                StatusCode = HttpStatusCode.NotFound
            };
        }
        
        public static CommentException AccessDenied()
        {
            return new CommentException
            {
                Code = "comment_access_denied",
                Message = "you don't have rights to execute this operation",
                StatusCode = HttpStatusCode.Unauthorized
            };
        }
        
        private CommentException(){}
    }
}