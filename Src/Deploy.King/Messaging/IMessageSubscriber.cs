namespace Deploy.King.Messaging
{
    public interface IMessageSubscriber
    {
        void AddListener(IMessageListener listener);
        void RemoveListener(IMessageListener listener);
    }
}