using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Deploy.Pawn.Api.Commands;
using Deploy.Pawn.Handlers;
using Topshelf;

namespace Deploy.Pawn
{
    class Program
    {
        static void Main(string[] args)
        {
            HostFactory.Run(x =>
            {
                x.Service<ServiceController>(s =>
                {
                    s.SetServiceName("deploypawn");
                    s.ConstructUsing(() => new ServiceController(new CommandHandlerFactory()));
                    s.WhenStarted(app => app.Start());
                    s.WhenStopped(app => app.Stop());
                });
                x.RunAsLocalSystem();

                x.SetDescription("Deploy Pawn - Recieves packages and task commands");
                x.SetDisplayName("Deploy Pawn Checker");
                x.SetServiceName("Deploy Pawn Checker");
            });
        }
    }

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
