namespace Deploy.Pawn.Api.Tasks
{
    public class RunExecutable : ITask<Result>
    {
        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }
    }
}