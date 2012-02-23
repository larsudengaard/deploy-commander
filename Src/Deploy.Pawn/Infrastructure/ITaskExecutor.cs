using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Tasks;

namespace Deploy.Pawn.Infrastructure
{
    public abstract class TaskExecutor<TTask, TResult> : ITaskExecutor<TTask, TResult>
        where TTask : ITask<TResult> where TResult : IResult
    {
        protected abstract TResult Execute(TTask task);

        public IResult Execute(ITask task)
        {
            return Execute((TTask)task);
        }
    }

    public interface ITaskExecutor<in TTask, TResult> : ITaskExecutor
        where TTask : ITask<TResult>
        where TResult : IResult
    {
    }

    public interface ITaskExecutor
    {
        IResult Execute(ITask task);
    }
}