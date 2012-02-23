using Deploy.Pawn.Api.Tasks;

namespace Deploy.Pawn.Infrastructure
{
    public interface ITaskExecuterFactory
    {
        ITaskExecutor CreateExecuterFor(ITask task);
    }
}