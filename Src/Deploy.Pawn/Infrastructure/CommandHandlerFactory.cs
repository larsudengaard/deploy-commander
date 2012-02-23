using System;
using System.Linq;
using Deploy.Pawn.Api.Commands;
using Deploy.Pawn.Handlers;

namespace Deploy.Pawn.Infrastructure
{
    internal class CommandHandlerFactory : ICommandHandlerFactory
    {
        public ICommandHandler CreateHandlerFor(ICommand command)
        {
            Type genericCommandHandler = typeof (ICommandHandler<>).MakeGenericType(command.GetType());
            var type = typeof(StopWebsiteHandler).Assembly.GetTypes().Single(genericCommandHandler.IsAssignableFrom);
            return (ICommandHandler)Activator.CreateInstance(type);
        }
    }
}