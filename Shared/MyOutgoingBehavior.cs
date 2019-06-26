using System;
using System.Threading;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

namespace Shared
{
    public class MyOutgoingBehavior : Behavior<IOutgoingLogicalMessageContext>
    {
        /// <summary>
        /// HACK: Gets value from ActionFilter, in host, null.
        /// </summary>
        public static AsyncLocal<SharedContext> ScopedSharedContext = new AsyncLocal<SharedContext>();

        public override async Task Invoke(IOutgoingLogicalMessageContext context, Func<Task> next)
        {
            //context.Builder would not have the scope of SharedContext in AspNetCore:
            var sharedContext = ScopedSharedContext.Value ?? context.Builder.Build<SharedContext>();

            context.Headers.Add("ContextId", sharedContext.ContextId);

            await next()
                .ConfigureAwait(false);
        }
    }
}
