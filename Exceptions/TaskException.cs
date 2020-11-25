using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace project_managment.Exceptions
{
    public class TaskException : BaseException
    {
        public static TaskException NotFound()
        {
            return new TaskException
            {
                Code = "task_not_found",
                Message = "task doesn't exist",
                StatusCode = HttpStatusCode.NotFound
            };
        }

        public static TaskException AccessDenied()
        {
            return new TaskException
            {
                Code = "task_access_denied",
                Message = "you can't access this task",
                StatusCode = HttpStatusCode.Unauthorized                
            };
        }

        public static TaskException PostFailed()
        {
            return new TaskException
            {
                Code = "task_save_failed",
                Message = "unable to save the task",
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public static TaskException LinkFailed()
        {
            return new TaskException
            {
                Code = "task_user_link_failed",
                Message = "unable to link user and task",
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        private TaskException()
        {
        }
    }
}