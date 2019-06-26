using System;
using System.Threading.Tasks;
using NServiceBus;
using Shared;

namespace HostApplication
{
    public class MyHandler : IHandleMessages<MyFirstMessage>, IHandleMessages<MySecondMessage>
    {
        private readonly SharedContext _sharedContext;

        public MyHandler(SharedContext sharedContext)
        {
            _sharedContext = sharedContext;
        }

        public async Task Handle(MyFirstMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"First - ContextId: {_sharedContext.ContextId}, MessageId: {message.MessageId}");

            await context.Send("Host", new MySecondMessage
            {
                MessageId = message.MessageId
            }).ConfigureAwait(false);
        }

        public Task Handle(MySecondMessage message, IMessageHandlerContext context)
        {
            Console.WriteLine($"Second - ContextId: {_sharedContext.ContextId}, MessageId: {message.MessageId}");

            return Task.CompletedTask;
        }
    }
}
