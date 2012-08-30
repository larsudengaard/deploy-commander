namespace Deploy.Commander.Application.Messaging
{
    public interface IMessageListener
    {
        void HandleMessage(Message message);
    }
}