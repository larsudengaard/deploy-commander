namespace Deploy.Pawn.Api.Tasks
{
    public class Unpack : ITask<Unpack.Result>
    {
        public class Result : IResult
        {
            public string PackagePath { get; set; }
        }

        public string PackageName { get; set; }
        public byte[] FileData { get; set; }
    }
}