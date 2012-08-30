using System;
using Deploy.Commander.Application.Messaging;

namespace Deploy.Commander.Infrastructure.Poll
{
    public class PollChannel : IMessageListener, IDisposable
    {
        readonly IMessageSubscriber messageSubscriber;
        readonly string channel;
        readonly string group;

        public PollChannel(IMessageSubscriber messageSubscriber, string channel, string @group)
        {
            this.messageSubscriber = messageSubscriber;
            messageSubscriber.AddListener(this);
            this.channel = channel;
            this.group = group;
        }

        public void HandleMessage(Commander.Application.Messaging.Message message)
        {
            Bus.Publish(new Message(channel, group, message));
        }

        public void Dispose()
        {
            messageSubscriber.RemoveListener(this);
        }
    }
}