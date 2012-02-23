namespace Deploy.Pawn.Api.Tasks
{
    public class StopWebsite : ManageWebsiteTask<StopWebsite.Result>
    {
        public class Result : IResult
        {
            public string Message { get; set; }

        }
    }
}