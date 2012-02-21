namespace Deploy.Pawn.Api.Commands
{
    public class Package : ICommand
    {
        public string PackageName { get; set; }
        public byte[] FileData { get; set; }
    }
}