using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.Soldier.Application;
using Deploy.Soldier.Application.Infrastructure;

namespace Deploy.Soldier.Infrastructure.Installers
{
    public class PawnServiceInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IPawnService>().ImplementedBy<PawnService>().LifeStyle.Transient);
            container.Register(Component.For<ITaskExecuterFactory>().ImplementedBy<TaskExecuterFactory>().LifeStyle.Transient);
        }
    }
}