namespace Deploy.Tasks.Executables
{
    public class StopProcess : ITask<StopProcess.Result>
    {
        public class Result : IResult
        {
            public bool Success { get; set; }
            public string ErrorMessage { get; set; }
        }

        public string Name { get; set; }
    }
}