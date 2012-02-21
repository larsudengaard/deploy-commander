using Deploy.Pawn.Api;
using Deploy.Pawn.Api.Commands;

namespace Deploy.Pawn
{
    public abstract class CommandHandler<TCommand> : ICommandHandler<TCommand> where TCommand : ICommand
    {
        protected abstract Result Handle(TCommand command);

        public Result Handle(ICommand command)
        {
            return Handle((TCommand) command);
        }
    }

    public interface ICommandHandler<in TCommand> : ICommandHandler where TCommand : ICommand
    {
    }

    public interface ICommandHandler
    {
        Result Handle(ICommand command);
    }
}