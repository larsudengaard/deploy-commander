namespace Deploy.Procedures.Messaging
{
    public interface IMessenger
    {
        void Publish(string message);
    }
}