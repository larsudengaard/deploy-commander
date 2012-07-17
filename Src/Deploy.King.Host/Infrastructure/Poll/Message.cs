using System;
using Deploy.King.Messaging;
using Deploy.Procedures.Messaging;

namespace Deploy.King.Host.Infrastructure.Poll
{
    public class Message
    {
        public Message(string channel, string @group, object data)
        {
            Channel = channel;
            Group = group;
            Data = data;
        }

        public string Channel { get; set; }
        public string Group { get; set; }
        public object Data { get; set; }
    }

    public interface IPollMessengerFactory
    {
        PollMessenger CreateMessenger(string projectId, string buildId);
    }

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