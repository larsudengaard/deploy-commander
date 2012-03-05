namespace Deploy.King.Messaging
{
    public interface IMessenger
    {
        void Publish(string message);
    }
}