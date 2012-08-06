using Deploy.Tasks;

namespace Deploy.Soldier.Application.Infrastructure
{
    public interface ITaskExecuterFactory
    {
        ITaskExecutor CreateExecuterFor(ITask task);
    }
}