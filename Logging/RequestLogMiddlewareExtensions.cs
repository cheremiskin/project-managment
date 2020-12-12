using Microsoft.AspNetCore.Builder;

namespace project_managment.Logging
{
    public static class RequestLogMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLog(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLogMiddleware>();
        }
    }
}