using System.Collections;
using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Deploy.King.Builds;
using Deploy.King.Executor;
using Deploy.King.Host.Infrastructure.Poll;
using Deploy.King.Messaging;
using Deploy.Procedures.Builds;
using Deploy.Procedures.Messaging;
using Deploy.Utilities;
using Castle.Facilities.TypedFactory;

namespace Deploy.King.Host.Infrastructure.Installers
{
    public class DeployKingInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<ProcedureExecutor>().LifeStyle.Transient);

            container.Register(Component.For<PollMessenger>().LifeStyle.Transient);
            container.Register(Component.For<IPollMessengerFactory>().AsFactory().LifeStyle.Transient);
            container.Register(Component.For<IMessenger, IMessageSubscriber>().ImplementedBy<Messenger>().LifeStyle.PerWebRequest);
            
            container.Register(Component.For<IBuildRepository>().ImplementedBy<TeamcityBuildRepository>().LifeStyle.Transient
                                   .DependsOn(new Hashtable
                                   {
                                       {"teamcityUrl", AppSettings.GetString("TeamcityUrl")},
                                       {"packagePath", AppSettings.GetPath("PackagePath")}
                                   }));
        }
    }
}