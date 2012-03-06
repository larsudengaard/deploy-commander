namespace Deploy.Pawn.Api.Tasks
{
    public class AddToLoadBalancer : ManageWebsiteTask<AddToLoadBalancer.Result>
    {
        public string WatchdogFilename { get; set; }

        public class Result : IResult
        {
        }
    }
}