namespace Deploy.Pawn.Api.Tasks
{
    public class CopyFolder : ITask<Result>
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
    }
}