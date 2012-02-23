namespace Deploy.Pawn.Api.Tasks
{
    public class ChangePsysicalPathOnWebsite : ManageWebsiteTask<ChangePsysicalPathOnWebsite.Result>
    {
        public class Result : IResult
        {
            public string OldPath { get; set; }
        }

        public string NewPath { get; set; }
    }
}