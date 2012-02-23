using Deploy.Pawn.Api.Tasks;

namespace Deploy.Pawn
{
    public interface ICommandHandlerFactory
    {
        ITaskExecuter CreateHandlerFor(ITask task);
    }
}