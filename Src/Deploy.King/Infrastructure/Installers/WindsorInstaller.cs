using Castle.Facilities.TypedFactory;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.King.Infrastructure.Factories;

namespace Deploy.King.Infrastructure.Installers
{
    public class WindsorInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<CreateGenericVersionOfInterfaceByArgumentsTypeSelector>().LifeStyle.Singleton);
        }
    }
}