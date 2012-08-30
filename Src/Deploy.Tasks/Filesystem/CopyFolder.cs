namespace Deploy.Tasks.Filesystem
{
    public class CopyFolder : ITask<CopyFolder.Result>
    {
        public class Result : IResult
        {
            public bool Success { get; set; }
            public string Message { get; set; }
        }

        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
    }
}