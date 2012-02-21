namespace Deploy.Pawn.Api.Commands
{
    public abstract class ManageWebsiteCommand : ICommand
    {
        public string WebsiteName { get; set; }
    }
}