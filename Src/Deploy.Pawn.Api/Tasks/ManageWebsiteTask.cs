namespace Deploy.Pawn.Api.Tasks
{
    public abstract class ManageWebsiteTask<TResult> : ITask<TResult>
        where TResult : IResult
    {
        public string WebsiteName { get; set; }
    }
}