namespace Deploy.Tasks.Websites
{
    public class AddToLoadBalancer : ManageWebsiteTask<AddToLoadBalancer.Result>
    {
        public string WatchdogFilename { get; set; }

        public class Result : IResult
        {
        }
    }
}