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
                Message = "task doesn't exist"
            };
        }

        public static TaskException AccessDenied()
        {
            return new TaskException
            {
                Code = "task_access_denied",
                Message = "you can't access this task"
            };
        }

        public static TaskException PostFailed()
        {
            return new TaskException
            {
                Code = "task_save_failed",
                Message = "unable to save the task"
            };
        }
    }
}