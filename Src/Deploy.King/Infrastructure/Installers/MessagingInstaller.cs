using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.Procedures.Messaging;

namespace Deploy.King.Infrastructure.Installers
{
    public class MessagingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<IMessenger, IMessageSubscriber>().ImplementedBy<Messenger>().LifeStyle.PerWebRequest);
        }
    }
}