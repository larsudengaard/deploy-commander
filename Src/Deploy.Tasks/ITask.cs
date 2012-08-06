namespace Deploy.Tasks
{
    public interface ITask<out TResult> : ITask
        where TResult : IResult
    {
    }

    public interface ITask
    {
    }
}