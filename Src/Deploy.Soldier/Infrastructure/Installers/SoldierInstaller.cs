using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.Soldier.Application;
using Deploy.Soldier.Application.Infrastructure;

namespace Deploy.Soldier.Infrastructure.Installers
{
    public class SoldierInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ISoldierService>().ImplementedBy<SoldierService>().LifeStyle.Transient);
            container.Register(Component.For<ITaskExecuterFactory>().ImplementedBy<TaskExecuterFactory>().LifeStyle.Transient);
        }
    }
}