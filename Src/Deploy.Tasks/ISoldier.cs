namespace Deploy.Tasks
{
    public interface ISoldier
    {
        TResult ExecuteTask<TResult>(ITask<TResult> task) where TResult : IResult;
    }
}