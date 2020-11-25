using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using project_managment.Exceptions;

namespace project_managment.Filters
{
    public class ExceptionFilter : ExceptionFilterAttribute
    {
        public override void OnException(ExceptionContext context)
        {
            if (context.Exception is BaseException e)
            {
                context.Result = new ObjectResult(new {code = e.Code, message = e.Message})
                {
                    StatusCode = (int) e.StatusCode
                };
            }
        }
    }
}