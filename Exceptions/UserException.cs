namespace project_managment.Exceptions
{
    public class UserException : BaseException
    {
        public static UserException NotFound()
        {
            return new UserException
            {
                Code = "user_not_found",
                Message = "user doesn't exist"
            };
        }
        public static UserException CreationDenied()
        {
            return new UserException
            {
                Code = "user_creation_denied",
                Message = "you can't create a user"
            };
        }

        public static UserException CreationFailed()
        {
            return new UserException
            {
                Code = "user_creation_failed",
                Message = "invalid data"
            };
        }

        public static UserException DeletionDenied()
        {
            return new UserException
            {
                Code = "user_deletion_failed",
                Message = "you can't delete this user" 
            };
        }
    }
}