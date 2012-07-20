using Deploy.Pawn.Api.Tasks;

namespace Deploy.Pawn.Api
{
    public interface IPawnClient
    {
        TResult ExecuteTask<TResult>(ITask<TResult> task) where TResult : IResult;
    }
}