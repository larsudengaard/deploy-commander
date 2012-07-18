using System;
using Deploy.King.Messaging;

namespace Deploy.King.Host.Infrastructure.Poll
{
    public class PollMessenger : IMessageListener, IDisposable
    {
        readonly IMessageSubscriber messageSubscriber;
        readonly string projectId;
        readonly string buildId;

        public PollMessenger(IMessageSubscriber messageSubscriber, string projectId, string buildId)
        {
            this.messageSubscriber = messageSubscriber;
            this.projectId = projectId;
            this.buildId = buildId;
            messageSubscriber.AddListener(this);
        }

        public void HandleMessage(Messaging.Message message)
        {
            Bus.Publish(new Message(projectId, buildId, message));
        }

        public void Dispose()
        {
            messageSubscriber.RemoveListener(this);
        }
    }
}