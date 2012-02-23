namespace Deploy.Pawn.Api.Tasks
{
    public class ChangePhysicalPathOnWebsite : ManageWebsiteTask<ChangePhysicalPathOnWebsite.Result>
    {
        public class Result : IResult
        {
            public string OldPath { get; set; }
        }

        public string NewPath { get; set; }
    }
}