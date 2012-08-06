namespace Deploy.Tasks.Websites
{
    public class RemoveFromLoadBalancer : ManageWebsiteTask<RemoveFromLoadBalancer.Result>
    {
        public string WatchdogFilename { get; set; }

        public class Result : IResult
        {
        }
    }
}