namespace Deploy.Tasks.Websites
{
    public class StopWebsite : ManageWebsiteTask<StopWebsite.Result>
    {
        public class Result : IResult
        {
            public string Message { get; set; }

        }
    }
}