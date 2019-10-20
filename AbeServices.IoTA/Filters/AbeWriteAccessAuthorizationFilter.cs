using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using AbeServices.IoTA.Services;

namespace AbeServices.IoTA.Filters
{
    public class AbeWriteAccessAuthorizationFilter : IAsyncActionFilter
    {
        private static string SessionHeader = "X-Session";
        private readonly IFiwareService _fiwareService;

        public AbeWriteAccessAuthorizationFilter(IFiwareService fiwareService)
        {
            _fiwareService = fiwareService;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var entityName = (string)context.ActionArguments["name"];

            context.HttpContext.Request.Headers.TryGetValue(SessionHeader, out var sessionIdValue);
            var sessionIdParam = sessionIdValue.Count > 0 ? sessionIdValue[0] : null;

            // get hmac from header

            context.HttpContext.Request.EnableBuffering();
            var bodyLength = (int)context.HttpContext.Request.ContentLength;
            var body = new byte[bodyLength];
            await context.HttpContext.Request.Body.ReadAsync(body, 0, bodyLength);

            var (authResult, sessionId) = await _fiwareService.Authorize(entityName, sessionIdParam, body, false);
            context.HttpContext.Response.Headers.Add(SessionHeader, sessionId.ToString());
            if (authResult != null)
            {
                context.Result = new FileContentResult(authResult, "application/octet-stream");
            }
            else
            {
                await next();
            }
        }
    }

    public class AbeWriteAccessAuthorizationAttribute : TypeFilterAttribute
    {
        public AbeWriteAccessAuthorizationAttribute()
            : base(typeof(AbeWriteAccessAuthorizationFilter)) { }
    }
}