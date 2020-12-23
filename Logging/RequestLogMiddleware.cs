using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Logging;

namespace project_managment.Logging
{
    public class RequestLogMiddleware 
    {
        private RequestDelegate _next;
        private ILogger<RequestLogMiddleware> _log;

        public RequestLogMiddleware(ILoggerFactory loggerFactory, RequestDelegate next)
        {
            _next = next; 
            _log = loggerFactory.CreateLogger<RequestLogMiddleware>();
        }
        

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            finally
            {
                if (context.Request != null && context.Request.Path.Value.StartsWith("/api"))
                    _log.LogInformation("Request {method} {url} => {statusCode}",
                    context.Request?.Method,
                    context.Request?.Path.Value,
                    context.Response?.StatusCode);
            }
        }
    }
}