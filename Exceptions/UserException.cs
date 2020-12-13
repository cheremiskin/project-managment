using System.Net;

namespace project_managment.Exceptions
{
    public class UserException : BaseException
    {
        public static UserException NotFound()
        {
            return new UserException
            {
                Code = "user_not_found",
                Message = "user doesn't exist",
                StatusCode = HttpStatusCode.NotFound
            };
        }
        public static UserException CreationDenied()
        {
            return new UserException
            {
                Code = "user_creation_denied",
                Message = "you can't create a user",
                StatusCode = HttpStatusCode.Unauthorized
            };
        }

        public static UserException CreationFailed()
        {
            return new UserException
            {
                Code = "user_creation_failed",
                Message = "invalid data",
                StatusCode = HttpStatusCode.BadRequest
            };
        }

        public static UserException DeletionDenied()
        {
            return new UserException
            {
                Code = "user_deletion_failed",
                Message = "you can't delete this user" ,
                StatusCode =  HttpStatusCode.Unauthorized
            };
        }

        public static UserException AccessDenied()
        {
            return new UserException
            {
                Code = "access_denied",
                Message = "you can't access this data",
                StatusCode = HttpStatusCode.Unauthorized
            };
        }
        private UserException (){}
    }
}