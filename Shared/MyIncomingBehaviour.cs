using System;
using System.Threading.Tasks;
using NServiceBus.Pipeline;

namespace Shared
{
    public class MyIncomingBehaviour : Behavior<IIncomingLogicalMessageContext>
    {
        public override async Task Invoke(IIncomingLogicalMessageContext context, Func<Task> next)
        {
            //This will be scoped correctly in hosts:
            var sharedContext = context.Builder.Build<SharedContext>();

            sharedContext.ContextId = context.Headers["ContextId"];

            await next()
                .ConfigureAwait(false);
        }
    }
}
