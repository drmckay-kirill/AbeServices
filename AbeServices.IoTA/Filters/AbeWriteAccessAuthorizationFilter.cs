using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using AbeServices.IoTA.Services;

namespace AbeServices.IoTA.Filters
{
    public class AbeWriteAccessAuthorizationFilter : IActionFilter
    {
        private readonly IFiwareService _fiwareService;

        public AbeWriteAccessAuthorizationFilter(IFiwareService fiwareService)
        {
            _fiwareService = fiwareService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            var entityName = (string)context.ActionArguments["name"];
            
            context.HttpContext.Request.Headers.TryGetValue("X-Session", out var sessionIdValue);
            var sessionId = sessionIdValue.Count > 0 ? sessionIdValue[0] : null;

            var bodyLength = (int)context.HttpContext.Request.Body.Length;
            var body = new byte[bodyLength];
            context.HttpContext.Request.Body.Read(body, 0, bodyLength);

            var authResult = _fiwareService.Authorize(entityName, sessionId, body, false);
            if (authResult != null)
                context.Result = new FileContentResult(authResult, "application/octet-stream");
        }

        public void OnActionExecuted(ActionExecutedContext context) { }
    }

    public class AbeWriteAccessAuthorizationAttribute : TypeFilterAttribute
    {
        public AbeWriteAccessAuthorizationAttribute()
            : base(typeof(AbeWriteAccessAuthorizationFilter)) { }
    }
}