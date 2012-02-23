namespace Deploy.Pawn.Api.Tasks
{
    public class DeleteFolder : ITask<Result>
    {
        public string Path { get; set; }
    }
}