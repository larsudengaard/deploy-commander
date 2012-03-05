namespace Deploy.Pawn.Api.Tasks
{
    public class DeleteFolder : ITask<DeleteFolder.Result>
    {
        public class Result : IResult
        {
        }

        public string Path { get; set; }
    }
}