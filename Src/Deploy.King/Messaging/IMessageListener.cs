namespace Deploy.King.Messaging
{
    public interface IMessageListener
    {
        void HandleMessage(Message message);
    }
}