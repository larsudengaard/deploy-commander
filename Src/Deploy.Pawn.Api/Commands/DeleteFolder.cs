namespace Deploy.Pawn.Api.Commands
{
    public class DeleteFolder : ICommand
    {
        public string Path { get; set; }
    }
}