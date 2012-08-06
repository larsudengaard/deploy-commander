namespace Deploy.Commander.Application.Messaging
{
    public interface IMessageSubscriber
    {
        void AddListener(IMessageListener listener);
        void RemoveListener(IMessageListener listener);
    }
}