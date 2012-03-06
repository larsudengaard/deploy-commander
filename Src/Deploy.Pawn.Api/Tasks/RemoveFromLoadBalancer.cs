namespace Deploy.Pawn.Api.Tasks
{
    public class RemoveFromLoadBalancer : ManageWebsiteTask<RemoveFromLoadBalancer.Result>
    {
        public string WatchdogFilename { get; set; }

        public class Result : IResult
        {
        }
    }
}