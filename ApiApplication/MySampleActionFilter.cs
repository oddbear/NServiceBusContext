using System;
using Microsoft.AspNetCore.Mvc.Filters;
using Shared;

namespace ApiApplication
{
    public class MySampleActionFilter : ActionFilterAttribute
    {
        private readonly SharedContext _sharedContext;

        public MySampleActionFilter(SharedContext sharedContext)
        {
            _sharedContext = sharedContext;

            //HACK:
            MyOutgoingBehavior.ScopedSharedContext.Value = sharedContext;
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var contextId = Guid.NewGuid().ToString();

            Console.WriteLine($"Generated ContextId '{contextId}', this might also be picked from headers, query params or similar.");

            _sharedContext.ContextId = contextId;
        }
    }
}
