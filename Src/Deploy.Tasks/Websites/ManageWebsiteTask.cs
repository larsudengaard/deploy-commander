namespace Deploy.Tasks.Websites
{
    public abstract class ManageWebsiteTask<TResult> : ITask<TResult>
        where TResult : IResult
    {
        public string WebsiteName { get; set; }
    }
}