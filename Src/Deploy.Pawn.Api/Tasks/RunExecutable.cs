namespace Deploy.Pawn.Api.Tasks
{
    public class RunExecutable : ITask<RunExecutable.Result>
    {
        public class Result : IResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
            public string ErrorMessage { get; set; }
        }

        public string ExecutablePath { get; set; }
        public string Arguments { get; set; }
    }
}