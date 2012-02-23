using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.Pawn
{
    public abstract class TaskExecuter<TTask, TResult> : ITaskExecuter<TTask, TResult>
        where TTask : ITask<TResult> where TResult : IResult
    {
        protected abstract TResult Handle(TTask task);

        public IResult Handle(ITask task)
        {
            return Handle((TTask)task);
        }
    }

    public interface ITaskExecuter<in TTask, TResult> : ITaskExecuter
        where TTask : ITask<TResult>
        where TResult : IResult
    {
    }

    public interface ITaskExecuter
    {
        IResult Handle(ITask task);
    }
}