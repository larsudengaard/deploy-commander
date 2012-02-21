using Deploy.Pawn.Api.Commands;

namespace Deploy.Pawn
{
    public interface ICommandHandlerFactory
    {
        ICommandHandler CreateHandlerFor(ICommand command);
    }
}