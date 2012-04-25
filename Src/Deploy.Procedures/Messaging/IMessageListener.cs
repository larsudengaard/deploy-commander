namespace Deploy.Procedures.Messaging
{
    public interface IMessageListener
    {
        void HandleMessage(Message message);
    }
}