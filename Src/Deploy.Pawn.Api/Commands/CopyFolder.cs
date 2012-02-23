namespace Deploy.Pawn.Api.Commands
{
    public class CopyFolder : ICommand
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
    }
}