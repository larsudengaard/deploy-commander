using Deploy.Pawn.Infrastructure;
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
                    s.ConstructUsing(() => new ServiceController(new TaskExecuterFactory()));
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
}
