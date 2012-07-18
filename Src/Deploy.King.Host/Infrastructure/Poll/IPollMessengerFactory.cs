namespace Deploy.King.Host.Infrastructure.Poll
{
    public interface IPollMessengerFactory
    {
        PollMessenger CreateMessenger(string projectId, string buildId);
    }
}