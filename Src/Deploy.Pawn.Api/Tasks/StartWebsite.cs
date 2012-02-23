namespace Deploy.Pawn.Api.Tasks
{
    public class StartWebsite : ManageWebsiteTask<StartWebsite.Result>
    {
        public class Result : IResult
        {
            public string Message { get; set; }
        }
    }
}