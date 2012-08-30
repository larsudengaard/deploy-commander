namespace Deploy.Tasks.Executables
{
    public class StartProcess : ITask<StartProcess.Result>
    {
        public class Result : IResult
        {
        }

        public string ProcessPath { get; set; }
        public string Arguments { get; set; }
    }
}