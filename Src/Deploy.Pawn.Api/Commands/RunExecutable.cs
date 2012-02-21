namespace Deploy.Pawn.Api.Commands
{
    public class RunExecutable : ICommand
    {
        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }
    }
}