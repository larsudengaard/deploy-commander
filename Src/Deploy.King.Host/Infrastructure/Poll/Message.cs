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
}