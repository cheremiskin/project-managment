using System;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using pm.Models;

namespace project_managment.Exceptions
{
    public class ProjectException : BaseException
    {
        public static ProjectException NotFound()
        {
            return new ProjectException
            {
                Code = "project_not_found",
                Message = "project doesn't exist"
            }; 
        }

        public static ProjectException AccessDenied()
        {
            return new ProjectException
            {
                Code = "access_denied",
                Message = "you can't access this project"
            }; 
        }

        public static ProjectException DeletionDenied()
        {
            return new ProjectException
            {
                Code = "deletion_denied",
                Message = "you can't delete project you don't own"
            };
        }

        public static ProjectException UpdateDenied()
        {
            return new ProjectException
            {
                Code = "update_denied",
                Message = "you can't update project you don't own"
            };
        }
        
        public static ProjectException AddMemberDenied()
        {
            return new ProjectException
            {
                Code = "add_member_denied",
                Message = "you can't update project you don't own"
            };
        }
        public static ProjectException DeleteMemberDenied()
        {
            return new ProjectException
            {
                Code = "delete_member_denied",
                Message = "you can't update project you don't own"
            };
        }

        public static ProjectException UserProjectLinkDeletionFailed()
        {
            return new ProjectException
            {
                Code = "unlink_failed",
                Message = "unable to unlink (maybe link doesn't exist)"
            };
        }

        public static ProjectException UserProjectLinkCreationFailed()
        {
            return new ProjectException
            {
                Code = "link_failed",
                Message = "unable to link (maybe link already exists)"
            };
        }
    }
}