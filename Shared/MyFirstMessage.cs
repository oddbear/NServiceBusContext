using NServiceBus;

namespace Shared
{
    public class MyFirstMessage : ICommand
    {
        public int MessageId { get; set; }
    }

    public class MySecondMessage : ICommand
    {
        public int MessageId { get; set; }
    }
}
